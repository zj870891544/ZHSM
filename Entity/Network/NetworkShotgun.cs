using UnityEditor;
using Mirror;
using Sirenix.OdinInspector;
using Unity.XR.PXR;
using UnityEngine;

namespace ZHSM
{
    public class NetworkShotgun : NetworkTwoHandedWeapon
    {
        [Header("霰弹枪设置")]
        [SerializeField] private int pelletCount = 8;       // 单次射击的弹丸数量
        [SerializeField] private float spreadAngle = 30f;   // 散射角度
        [SerializeField] private float reloadTime = 1.0f;   // 装弹时间
        [SerializeField] private float fireDistance = 15f;
        [SerializeField] private float recoilStrength = 1.2f; // 后坐力强度

        private bool canFire = true;        // 是否可以开火
        private float reloadTimer = 0f;     // 装弹计时器

        [Button]
        public void DebugFire()
        {
            OnTrigger();
        }

        protected override void Update()
        {
            base.Update();

            // 处理装弹冷却
            if (!canFire && isServer)
            {
                reloadTimer += Time.deltaTime;
                if (reloadTimer >= reloadTime)
                {
                    canFire = true;
                    reloadTimer = 0f;
                }
            }
        }

        protected override void OnTrigger()
        {
            base.OnTrigger();

            if (!isOwned)
            {
                Debug.LogWarning("Weapon is not owner.");
                return;
            }

            if (!canFire)
            {
                return;
            }

            // 霰弹枪有较强烈的后坐力，震动幅度更大
            // if (hapticEnabled)
            // {
            //     PXR_Input.SendHapticImpulse(PXR_Input.VibrateType.RightController,
            //         amplitude * 1.5f,
            //         duration * 2,
            //         frequency);
            // }
            // 应用后坐力效果，霰弹枪后坐力较大
            ApplyRecoil(recoilStrength);

            // 播放射击效果并发送网络命令
            ShowShotgunBlast(isServer);
            CmdFire();

            // 本地进入装弹状态
            canFire = false;
            reloadTimer = 0f;
        }

        [Command(requiresAuthority = false)]
        public void CmdFire(NetworkConnectionToClient sender = null)
        {
            if (!canFire)
                return;

            // 服务器端逻辑
            if (!isOwned)
            {
                ShowShotgunBlast(true);
            }

            // 广播给其他客户端
            RpcOtherClientsFire();

            // 服务器设置装弹冷却
            canFire = false;
            reloadTimer = 0f;
        }

        [ClientRpc]
        public void RpcOtherClientsFire()
        {
            // 其他客户端执行，本地和服务器已执行过，忽略
            if (isOwned || isServer) return;

            ShowShotgunBlast(false);
        }

        private void ShowShotgunBlast(bool isServerBullet)
        {
            // 播放开火音效
            GameEntry.Sound.PlaySound(weaponCfg.FireSounds, firePoint.position);

            // 播放开火特效
            if (weaponCfg.FlashEffectId > 0)
            {
                GameEntry.Entity.ShowEffect(new EffectData(GameEntry.Entity.GenerateSerialId(), weaponCfg.FlashEffectId)
                {
                    Position = firePoint.position,
                    Rotation = firePoint.rotation
                });
            }

            // 生成多个散射弹丸
            for (int i = 0; i < pelletCount; i++)
            {
                // 计算随机散射方向
                Vector3 spreadDirection = GetSpreadDirection(firePoint.forward);
                Quaternion spreadRotation = Quaternion.LookRotation(spreadDirection);

                // 每个弹丸的伤害是武器基础伤害除以弹丸数量，但总伤害略高
                float pelletDamage = weaponCfg.Damage * 1.5f / pelletCount;

                // 生成子弹实体
                GameEntry.Entity.ShowBullet(new BulletData(GameEntry.Entity.GenerateSerialId(), weaponCfg.BulletId,
                    CampType.Player, NetworkServer.active)
                {
                    Position = firePoint.position,
                    Rotation = spreadRotation,
                    Owner = gameObject,
                    KeepTime = 5.0f,  // 霰弹的生存时间较短
                    Speed = 30.0f,    // 霰弹速度适中
                    Damage = (int)pelletDamage,
                    KnockUp = weaponCfg.KnockUp / pelletCount,
                    HitEffectId = weaponCfg.HitEffectId,
                    HitSoundId = GameEntry.Sound.GetRandomSoundId(weaponCfg.HitSounds)
                });
            }

            // 播放装弹动画(如果有)
           
        }

        // 计算随机散射方向
        private Vector3 GetSpreadDirection(Vector3 forwardDirection)
        {
            // 在一个锥体内随机生成方向
            float angleRad = spreadAngle * Mathf.Deg2Rad;
            float randomAngle = Random.Range(0f, 2f * Mathf.PI);
            float randomRadius = Random.Range(0f, Mathf.Sin(angleRad));

            // 计算随机点在单位球上的位置
            Vector3 randomPoint = new Vector3(
                randomRadius * Mathf.Cos(randomAngle),
                randomRadius * Mathf.Sin(randomAngle),
                Mathf.Cos(angleRad)
            );

            // 将随机点转换为世界坐标系方向
            Vector3 forward = forwardDirection.normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            Vector3 up = Vector3.Cross(forward, right);

            return forward * randomPoint.z + right * randomPoint.x + up * randomPoint.y;
        }

        // 在场景中绘制散射范围(仅在编辑器中)
        private void OnDrawGizmosSelected()
        {
            if (firePoint == null) return;

            Gizmos.color = Color.yellow;
            float distance = 5f;
            float radius = distance * Mathf.Tan(spreadAngle * Mathf.Deg2Rad);

            Vector3 endPoint = firePoint.position + firePoint.forward * distance;

            // 如果启用了双手持握稳定性，绘制更小的散射范围
            if (secondaryGrabPoint != null)
            {
                Gizmos.color = Color.green;
                float reducedSpreadAngle = spreadAngle * (1.0f - stabilityBonus);
                float reducedRadius = distance * Mathf.Tan(reducedSpreadAngle * Mathf.Deg2Rad);
                DrawCircle(endPoint, firePoint.forward, reducedRadius, 32);
            }
            
            // 绘制散射锥体
            Gizmos.DrawLine(firePoint.position, endPoint);
            DrawCircle(endPoint, firePoint.forward, radius, 32);
        }

        private void DrawCircle(Vector3 center, Vector3 normal, float radius, int segments)
        {
            // 创建锥体底面的圆
            Vector3 forward = normal.normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            if (right.magnitude < 0.01f)
                right = Vector3.Cross(Vector3.forward, forward).normalized;
            Vector3 up = Vector3.Cross(forward, right);

            Vector3 prevPoint = center + right * radius;
            for (int i = 1; i <= segments; i++)
            {
                float angle = i * 2 * Mathf.PI / segments;
                Vector3 nextPoint = center + right * radius * Mathf.Cos(angle) + up * radius * Mathf.Sin(angle);
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }
    }
}
