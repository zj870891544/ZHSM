namespace ZHSM
{
    [System.Serializable]
    public class B2LPlayerEntityData : EntityData
    {
        private string m_PlayerId;
        private float m_RemoveDeley = 3.0f;
        
        public B2LPlayerEntityData(int entityId, int typeId) : base(entityId, typeId)
        {
            
        }
        
        public string PlayerId { get { return m_PlayerId; } set { m_PlayerId = value; } }
        public float RemoveDeley { get { return m_RemoveDeley; } set { m_RemoveDeley = value; } }
    }
}