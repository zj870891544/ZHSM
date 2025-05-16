using UnityEngine;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class BulletEntity : Entity
    {
        private BulletData m_BulletData;
        private Rigidbody m_RigidBody;
        private float m_ElapseSeconds = 0f;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_RigidBody = GetComponent<Rigidbody>();
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            m_BulletData = userData as BulletData;
            if (m_BulletData == null)
            {
                Log.Error("BulletData is null in BulletEntity's Show.");
                return;
            }

            Collider selfCollider = gameObject.GetComponent<Collider>();
            Collider[] ownerColliders = m_BulletData.Owner.GetComponentsInChildren<Collider>();
            foreach (Collider c in ownerColliders)
            {
                Physics.IgnoreCollision(selfCollider, c);
            }
            
            m_ElapseSeconds = 0f;
            m_RigidBody.velocity = transform.forward * m_BulletData.Speed;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            
            m_ElapseSeconds += elapseSeconds;
            if (m_ElapseSeconds >= m_BulletData.KeepTime)
            {
                GameEntry.Entity.HideEntity(this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out TargetableTrigger trigger))
            {
                if (m_BulletData.Camp == trigger.Camp) return;

                if (m_BulletData.IsDetermineDamage)
                {
                    trigger.TriggerDamage(new DamageRequestData
                    {
                        damage = m_BulletData.Damage,
                        knockUp = m_BulletData.KnockUp,
                        knockUpDirection = transform.forward
                    });
                }
            }
            
            DestroySelf();
        }

        private void DestroySelf()
        {
            //effect
            if(m_BulletData.HitEffectId > 0)
            {
                GameEntry.Entity.ShowEffect(new EffectData(GameEntry.Entity.GenerateSerialId(), m_BulletData.HitEffectId)
                {
                    Position = transform.position,
                    Rotation = transform.rotation
                });
            }

            //sound
            if(m_BulletData.HitSoundId > 0)
                GameEntry.Sound.PlaySound(m_BulletData.HitSoundId);
            
            GameEntry.Entity.HideEntity(this);
        }
    }
}