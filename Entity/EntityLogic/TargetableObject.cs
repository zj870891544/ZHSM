using Mirror;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public abstract class TargetableObject : Entity
    {
        private TargetableObjectData m_TargetableObjectData;
        private NetworkTargetable m_NetworkTargetable;
        private Transform m_HPBarPoint;
        private Transform m_DamageTextPoint;
        
        public bool IsDead => m_NetworkTargetable.IsDead;
        public Transform HPBarPoint => m_HPBarPoint;
        public Transform DamageTextPoint => m_DamageTextPoint;
        
        [ServerCallback]
        public void ApplyDamage(DamageRequestData damageRequestData)
        {
            if (m_NetworkTargetable.IsDead) return;
            
            m_NetworkTargetable.TakeDamage(damageRequestData.damage);
            if (m_NetworkTargetable.IsDead)
            {
                OnDead(damageRequestData);
                return;
            }
            
            OnDamage(damageRequestData);
        }

        public CampType Camp => m_NetworkTargetable.SyncedCamp;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            m_TargetableObjectData = userData as TargetableObjectData;
            if (m_TargetableObjectData == null)
            {
                Log.Error("TargetableObject data is invalid.");
                return;
            }

            m_NetworkTargetable = GetComponent<NetworkTargetable>();
            m_NetworkTargetable.SyncedCamp = m_TargetableObjectData.Camp;
            m_NetworkTargetable.SyncedHP = m_TargetableObjectData.HP;
            
            GameEntry.Level.AddTargetable(Camp, m_NetworkTargetable);

            if (NetworkServer.active)
                NetworkServer.Spawn(gameObject, m_NetworkTargetable.connectionToClient);
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            
            if(NetworkServer.active)
                NetworkServer.UnSpawn(gameObject);
        }

        protected virtual void OnDamage(DamageRequestData damageRequestData) { }

        protected virtual void OnDead(DamageRequestData damageRequestData)
        {
            GameEntry.Level.RemoveTargetable(Camp, m_NetworkTargetable);
        }
        
        
    }
}