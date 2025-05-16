namespace ZHSM
{
    public class DefenseTowerData : TargetableObjectData
    {
        private int m_MaxHP;
        private float m_Radius;
        private float m_OccupySpeed;
        
        public DefenseTowerData(int entityId, int typeId, CampType camp, int maxHp) : base(entityId, typeId, camp)
        {
            m_MaxHP = HP = maxHp;
        }

        public override int MaxHP => m_MaxHP;
        
        public float Radius { get => m_Radius; set => m_Radius = value; }
        public float OccupySpeed
        {
            get => m_OccupySpeed;
            set => m_OccupySpeed = value;
        }
    }
}