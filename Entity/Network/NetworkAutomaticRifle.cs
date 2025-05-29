using Mirror;
using Sirenix.OdinInspector;
using Unity.XR.PXR;
using UnityEngine;

namespace ZHSM
{
    public class NetworkAutomaticRifle : NetworkTwoHandedWeapon
    {
        [Button]
        [HorizontalGroup("TriggerGroup")]
        public void DebugTriggerOn()
        {
            isTriggered = true;
        }
        
        [Button]
        [HorizontalGroup("TriggerGroup")]
        public void DebugTriggerOff()
        {
            isTriggered = false;
        }
        
        [Header("自动步枪设置")]
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private float baseSpreadAngle = 3.0f; // 基础散布角度
        [SerializeField] private float recoilStrength = 0.7f; // 后坐力强度

        private float fireTimer = 0.0f;

        protected override void Update()
        {
            base.Update();

            if (isTriggered)
            {
                fireTimer += Time.deltaTime;
                if (fireTimer >= fireRate)
                {
                    fireTimer = 0.0f;
                    
                    ShowBullet(isServer);
                }
            }
        }

        protected override void OnTriggeredChanged(bool oldValue, bool newValue)
        {
            base.OnTriggeredChanged(oldValue, newValue);

            Debug.Log("triggered >>> " + newValue);

            fireTimer = 0.0f;
            
            if(newValue) ShowBullet(isServer);
        }

        private void ShowBullet(bool isServerBullet)
        {
            // if(hapticEnabled)
            //     PXR_Input.SendHapticImpulse(PXR_Input.VibrateType.RightController, amplitude, duration, frequency);
           
            // 应用后坐力效果
            ApplyRecoil(recoilStrength);
            
            // sound
            GameEntry.Sound.PlaySound(weaponCfg.FireSounds, firePoint.position);
            
            //effect
            if (weaponCfg.FlashEffectId > 0)
            {
                GameEntry.Entity.ShowEffect(new EffectData(GameEntry.Entity.GenerateSerialId(), weaponCfg.FlashEffectId)
                {
                    Position = firePoint.position,
                    Rotation = firePoint.rotation
                });
            }
            
            // 计算实际散布角度
           // float spreadAngle = CalculateSpreadAngle(baseSpreadAngle);
            
            // 根据散布角度计算子弹方向的随机偏移
          //  Vector3 bulletDirection = CalculateBulletDirection(spreadAngle);
            
            // fire bullet
            GameEntry.Entity.ShowBullet(new BulletData(GameEntry.Entity.GenerateSerialId(), weaponCfg.BulletId,
                CampType.Player, NetworkServer.active)
            {
                Position = firePoint.position,
                Rotation = firePoint.rotation,
                Owner = gameObject,
                KeepTime = 10.0f,
                Speed = 20.0f,
                Damage = weaponCfg.Damage,
                KnockUp = 0,
                HitEffectId = weaponCfg.HitEffectId,
                HitSoundId = GameEntry.Sound.GetRandomSoundId(weaponCfg.HitSounds)
            });
        }
        
        /// <summary>
        /// 计算子弹发射方向，考虑散布角度
        /// </summary>
        private Vector3 CalculateBulletDirection(float spreadAngle)
        {
            if (spreadAngle <= 0)
            {
                return firePoint.forward;
            }
            
            // 在锥形范围内随机生成方向
            float randAngle = Random.Range(0f, spreadAngle);
            float randDir = Random.Range(0f, 360f);
            
            // 将随机角度转换为方向向量
            Vector3 bulletDir = firePoint.forward;
            bulletDir = Quaternion.AngleAxis(randAngle, firePoint.up) * bulletDir;
            bulletDir = Quaternion.AngleAxis(randDir, firePoint.forward) * bulletDir;
            
            return bulletDir.normalized;
        }
    }
}