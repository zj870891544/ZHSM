using UnityEngine;

namespace ZHSM
{
    [System.Serializable]
    public abstract class TargetableObjectData : EntityData
    {
        [SerializeField] private CampType m_Camp = CampType.Unknown;
        [SerializeField] private int m_HP = 0;
        
        public TargetableObjectData(int entityId, int typeId, CampType camp) : base(entityId, typeId)
        {
            m_Camp = camp;
            m_HP = 0;
        }
        
        public CampType Camp => m_Camp;

        public int HP
        {
            get => m_HP;
            set => m_HP = value;
        }

        public abstract int MaxHP { get; }
    }
}