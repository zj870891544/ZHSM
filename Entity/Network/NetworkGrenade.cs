using Mirror;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class NetworkGrenade : NetworkBehaviour
    {
        [Header("手雷基础属性")]
        [SerializeField] private float fuseTime = 3.0f;  // 引信时间
        [SerializeField] private GameObject grenadeModel;
        [SerializeField] private GameObject trailEffect;
        [SerializeField] private LayerMask hitLayerMask;

        [SyncVar(hook = nameof(OnGrenadeTypeUpdate))]
        private int grenadeType;

        [SyncVar] private int damage;
        [SyncVar] private float explosionRadius;
        [SyncVar] private float effectDuration;
        [SyncVar] private int explosionEffectId;
        [SyncVar] private int explosionSoundId;

        private Rigidbody rb;
        private bool hasExploded = false;
        private Collider grenadeCollider;
        private float explodeTimer = 0f;
        private bool isArmed = false;
        private readonly List<GameObject> affectedTargets = new List<GameObject>();

        public GameObject Owner { get; set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            grenadeCollider = GetComponent<Collider>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            // 手雷在服务器上生成后开始计时
            isArmed = true;
        }

        private void Update()
        {
            if (isServer && isArmed && !hasExploded)
            {
                explodeTimer += Time.deltaTime;
                if (explodeTimer >= fuseTime)
                {
                    Explode();
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (isServer && isArmed && !hasExploded)
            {
                // 播放碰撞音效（如果有）
                // 如果是爆破手雷，可以考虑直接爆炸
                if ((int)GrenadeType.Explosive == grenadeType)
                {
                    Explode();
                }
            }
        }

        [TargetRpc]
        public void RpcSetGrenadeProperties(int _grenadeType, int _damage, float _explosionRadius,
            float _effectDuration, int _explosionEffectId, int _explosionSoundId)
        {
            grenadeType = _grenadeType;
            damage = _damage;
            explosionRadius = _explosionRadius;
            effectDuration = _effectDuration;
            explosionEffectId = _explosionEffectId;
            explosionSoundId = _explosionSoundId;

            OnGrenadeTypeUpdate(0, _grenadeType);
        }

        private void OnGrenadeTypeUpdate(int oldValue, int newValue)
        {
            // 根据手雷类型更新外观
            UpdateGrenadeAppearance((GrenadeType)newValue);
        }

        private void UpdateGrenadeAppearance(GrenadeType type)
        {
            if (grenadeModel == null) return;

            // 更新手雷模型颜色或材质
            Renderer renderer = grenadeModel.GetComponent<Renderer>();
            if (renderer != null)
            {
                switch (type)
                {
                    case GrenadeType.Explosive:
                        renderer.material.color = Color.red;
                        break;
                    case GrenadeType.Fire:
                        renderer.material.color = new Color(1.0f, 0.5f, 0.0f); // 橙色
                        break;
                    case GrenadeType.Ice:
                        renderer.material.color = Color.cyan;
                        break;
                }
            }

            // 更新尾迹效果
            if (trailEffect != null)
            {
                ParticleSystem ps = trailEffect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var main = ps.main;
                    switch (type)
                    {
                        case GrenadeType.Explosive:
                            main.startColor = Color.red;
                            break;
                        case GrenadeType.Fire:
                            main.startColor = new Color(1.0f, 0.5f, 0.0f);
                            break;
                        case GrenadeType.Ice:
                            main.startColor = Color.cyan;
                            break;
                    }
                }
            }
        }

        [Server]
        public void Launch(Vector3 direction, float force)
        {
            if (rb == null) return;

            rb.isKinematic = false;
            rb.AddForce(direction * force, ForceMode.Impulse);

            // 手雷被发射后立即激活
            isArmed = true;
            explodeTimer = 0f;
        }
        
        /// <summary>
        /// 使用指定速度发射手雷（用于抛物线发射）
        /// </summary>
        /// <param name="velocity">发射速度向量</param>
        [Server]
        public void LaunchWithVelocity(Vector3 velocity)
        {
            if (rb == null) return;
            
            rb.isKinematic = false;
            rb.velocity = velocity;
            
            // 手雷被发射后立即激活
            isArmed = true;
            explodeTimer = 0f;
        }

        [Server]
        private void Explode()
        {
            if (hasExploded) return;

            hasExploded = true;

            // 播放爆炸特效和音效
            RpcPlayExplosionEffects();

            // 检测爆炸范围内的目标并应用效果
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, hitLayerMask);

            foreach (var hitCollider in hitColliders)
            {
                GameObject targetObj = hitCollider.gameObject;

                // 防止重复处理同一个目标
                if (affectedTargets.Contains(targetObj)) continue;
                affectedTargets.Add(targetObj);

                // 查找可以受到伤害的组件
                NetworkTargetable target = targetObj.GetComponent<NetworkTargetable>();
                if (target != null && target.gameObject != Owner)
                { 
                    // 计算距离衰减
                    float distance = Vector3.Distance(transform.position, targetObj.transform.position);
                    float damagePercent = 1.0f - Mathf.Clamp01(distance / explosionRadius);

                    // 应用不同类型手雷的效果
                    ApplyGrenadeEffect(target, damagePercent);
                }
            }

            // 延迟销毁手雷
            StartCoroutine(DestroyAfterDelay(0.5f));
        }

        [Server]
        private void ApplyGrenadeEffect(NetworkTargetable target, float damagePercent)
        {
            int calculatedDamage = Mathf.RoundToInt(damage * damagePercent);

            switch ((GrenadeType)grenadeType)
            {
                case GrenadeType.Explosive:
                    // 爆破伤害
                    target.TakeDamage(calculatedDamage);
                    break;

                case GrenadeType.Fire:
                    // 火焰伤害（初始伤害 + 持续伤害）
                    target.TakeDamage(calculatedDamage / 2);

                    // 添加持续燃烧效果（如果目标有这样的接口）
                    // 这里假设目标有ApplyDamageOverTime方法
                    // if (target is IDamageOverTimeTarget dotTarget)
                    // {
                    //     dotTarget.ApplyDamageOverTime(calculatedDamage / 2, effectDuration, DamageType.Fire);
                    // }
                    break;

                case GrenadeType.Ice:
                    // 冰冻伤害（较小伤害 + 减速效果）
                    target.TakeDamage(calculatedDamage / 3);

                    // 添加减速效果（如果目标有这样的接口）
                    // 这里假设目标有ApplyMovementEffect方法
                    // if (target is IMovementEffectTarget moveTarget)
                    // {
                    //     moveTarget.ApplyMovementEffect(0.5f, effectDuration, EffectType.Slow);
                    // }
                    break;
            }
        }

        [ClientRpc]
        private void RpcPlayExplosionEffects()
        {
            // 隐藏手雷模型，显示爆炸效果
            if (grenadeModel != null)
                grenadeModel.SetActive(false);

            if (trailEffect != null)
                trailEffect.SetActive(false);

            if (grenadeCollider != null)
                grenadeCollider.enabled = false;

            // 播放爆炸音效
            GameEntry.Sound.PlaySound(explosionSoundId, transform.position);

            // 播放爆炸特效
            if (explosionEffectId > 0)
            {
                GameEntry.Entity.ShowEffect(new EffectData(GameEntry.Entity.GenerateSerialId(), explosionEffectId)
                {
                    Position = transform.position,
                    KeepTime = 3.0f
                });
            }

            // 根据不同类型的手雷播放不同的额外特效
            PlayTypeSpecificEffects();
        }

        private void PlayTypeSpecificEffects()
        {
            switch ((GrenadeType)grenadeType)
            {
                case GrenadeType.Explosive:
                    // 爆炸震动屏幕
                    // TODO: 添加屏幕震动效果
                    break;

                case GrenadeType.Fire:
                    // 火焰特效
                    // TODO: 添加持续燃烧地面特效
                    break;

                case GrenadeType.Ice:
                    // 冰冻特效
                    // TODO: 添加冰冻区域特效
                    break;
            }
        }

        private IEnumerator DestroyAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            // 移除实体
            //GameEntry.Entity.HideEntity(this.Entity);
        }
    }
}