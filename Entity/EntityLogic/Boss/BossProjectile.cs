using Telepathy;
using UnityEngine;

namespace ZHSM
{
    public class BossProjectile : Entity
    {
        private Rigidbody m_Rigidbody;
        private BossProjectileData m_ProjectileData;
        private bool m_IsTriggered = false;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_Rigidbody = GetComponent<Rigidbody>();
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            m_ProjectileData = userData as BossProjectileData;
            if (m_ProjectileData == null)
            {
                Log.Error("BossProjectileData is invalid.");
                return;
            }

            m_IsTriggered = false;
            m_Rigidbody.velocity = (m_ProjectileData.Destination - CachedTransform.position).normalized * m_ProjectileData.Speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (m_IsTriggered) return;
            m_IsTriggered = true;
            
            m_Rigidbody.velocity = Vector3.zero;
            GameEntry.Entity.ShowEffect(new EffectData(GameEntry.Entity.GenerateSerialId(), m_ProjectileData.HitEffect)
            {
                Position = CachedTransform.position,
                Rotation = Quaternion.identity
            });

            if (!m_ProjectileData.IsDetermineDamage)
            {
                Collider[] colliders = Physics.OverlapSphere(CachedTransform.position, m_ProjectileData.DamageRadius);
                foreach (Collider collider in colliders)
                {
                    if (collider.TryGetComponent(out TargetableTrigger trigger))
                    {
                        trigger.TriggerDamage(new DamageRequestData
                        {
                            damage = m_ProjectileData.Damage,
                            knockUp = 0,
                            knockUpDirection = transform.forward
                        });
                    }
                }
            }

            if (GameEntry.Entity.HasEntity(m_ProjectileData.IndicatorEntityId))
            {
                GameEntry.Entity.HideEntity(m_ProjectileData.IndicatorEntityId);
            }
            
            GameEntry.Entity.HideEntity(this);
        }
    }
}