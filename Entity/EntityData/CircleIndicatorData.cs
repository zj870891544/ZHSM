namespace ZHSM
{
    public class CircleIndicatorData : EntityData
    {
        private float m_Radius;
        
        public CircleIndicatorData(int entityId, int typeId) : base(entityId, typeId)
        {
            
        }
        
        public float Radius { get { return m_Radius; } set { m_Radius = value; } }
    }
}