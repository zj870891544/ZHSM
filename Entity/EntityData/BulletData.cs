using UnityEngine;

namespace ZHSM
{
    public class BulletData : EntityData
    {
        private bool m_IsDetermineDamage;
        private CampType m_Camp;

        public BulletData(int entityId, int typeId, CampType campType, bool isDetermineDamage) : base(entityId, typeId)
        {
            m_Camp = campType;
            m_IsDetermineDamage = isDetermineDamage;
        }

        public CampType Camp => m_Camp;
        public bool IsDetermineDamage => m_IsDetermineDamage;

        public GameObject Owner;
        public float KeepTime = 5.0f;
        public float Speed = 1000;
        
        public int Damage { get; set; }
        public float KnockUp { get; set; }
        public int HitEffectId { get; set; }
        public int HitSoundId { get; set; }
    }
}