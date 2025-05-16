using UnityEngine;
using UnityEngine.Serialization;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class TargetableTrigger : MonoBehaviour
    {
        [SerializeField] private TargetableObject m_Targetable;
        [SerializeField] private CampType m_Camp;
        
        public CampType Camp => m_Camp;

        private void Start()
        {
            m_Targetable = GetComponentInParent<TargetableObject>();
            
            NetworkTargetable networkTargetable = GetComponentInParent<NetworkTargetable>();
            m_Camp = networkTargetable.SyncedCamp;
        }

        public void TriggerDamage(DamageRequestData damageRequestData)
        {
            if (!m_Targetable)
            {
                Log.Error("TargetableTrigger's target not found.");
                return;
            }
            
            m_Targetable.ApplyDamage(damageRequestData);
        }
    }
}