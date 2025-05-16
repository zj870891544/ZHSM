using Mirror;
using Sirenix.OdinInspector;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.Serialization;

namespace ZHSM
{
    public class NetworkPistol : NetworkWeapon
    {
        [Button]
        public void DebugFire()
        {
            OnTrigger();
        }

        protected override void OnTrigger()
        {
            base.OnTrigger();
            
            if (!isOwned)
            {
                Debug.LogWarning("Weapon is not owner.");
                return;
            }

            if(hapticEnabled)
                PXR_Input.SendHapticImpulse(PXR_Input.VibrateType.RightController, amplitude, duration, frequency);

            // Debug.Log("local fire >>> ");
            ShowBullet(true);
            
            CmdFire();
        }
        
        [Command(requiresAuthority = false)]
        public void CmdFire(NetworkConnectionToClient sender = null)
        {
            if (!isOwned)
            {
                // Debug.Log("rpc server fire >>> ");
                ShowBullet(true);
            }
            
            RpcOtherClientsFire();
        }

        [ClientRpc]
        public void RpcOtherClientsFire()
        {
            // 本地和服务端已执行过，忽略
            if (isOwned || isServer) return;

            Debug.Log("rpc other clients fire >>> ");
            ShowBullet(false);
        }

        private void ShowBullet(bool isServerBullet)
        {
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
                KnockUp = weaponCfg.KnockUp,
                HitEffectId = weaponCfg.HitEffectId,
                HitSoundId = GameEntry.Sound.GetRandomSoundId(weaponCfg.HitSounds)
            });
        }
    }
}