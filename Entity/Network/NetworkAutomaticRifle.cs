using Mirror;
using Sirenix.OdinInspector;
using Unity.XR.PXR;
using UnityEngine;

namespace ZHSM
{
    public class NetworkAutomaticRifle : NetworkWeapon
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
        
        
        [SerializeField] private float fireRate = 0.5f;

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
            if(hapticEnabled)
                PXR_Input.SendHapticImpulse(PXR_Input.VibrateType.RightController, amplitude, duration, frequency);
            
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
    }
}