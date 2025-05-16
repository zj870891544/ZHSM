using Mirror;
using UnityEngine;

namespace ZHSM
{
    public class NetworkEnemy : NetworkTargetable
    {
        public override Vector3 Position => transform.position;
        
        [ClientRpc]
        public void RpcAttack(Vector3 firePosition, Quaternion fireRotation, int flashEffectId, int fireSoundId,
            int bulletId, int damage, int hitEffectId, int hitSoundId)
        {
            //effect
            if (flashEffectId > 0)
            {
                GameEntry.Entity.ShowEffect(new EffectData(GameEntry.Entity.GenerateSerialId(), flashEffectId)
                {
                    Position = firePosition,
                    Rotation = fireRotation
                });
            }

            // play sound
            if(fireSoundId > 0)
                GameEntry.Sound.PlaySound(fireSoundId, firePosition);

            // fire bullet
            GameEntry.Entity.ShowBullet(new BulletData(GameEntry.Entity.GenerateSerialId(), bulletId,
                CampType.Enemy, NetworkServer.active)
            {
                Position = firePosition,
                Rotation = fireRotation,
                Owner = gameObject,
                KeepTime = 10.0f,
                Speed = 20.0f,
                Damage = damage,
                KnockUp = 0,
                HitEffectId = hitEffectId,
                HitSoundId = hitSoundId
            });
        }

        [ClientRpc]
        public void RpcTakeDamage(int damageSoundId)
        {
            if(damageSoundId > 0)
                GameEntry.Sound.PlaySound(damageSoundId, transform.position);
        }

        [ClientRpc]
        public void RpcDead(int deadSoundId)
        {
            if(deadSoundId > 0)
                GameEntry.Sound.PlaySound(deadSoundId, transform.position);
        }
    }
}