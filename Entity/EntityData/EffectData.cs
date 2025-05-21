using UnityEngine;

namespace ZHSM
{
    [System.Serializable]
    public class EffectData : EntityData
    {
        [SerializeField] private float m_KeepTime = 0f;
        [SerializeField] private Transform m_AttachPoint;

        public EffectData(int entityId, int typeId)
            : base(entityId, typeId)
        {
            m_KeepTime = 3f;
        }

        public float KeepTime { get { return m_KeepTime; } set { m_KeepTime = value; } }
        public Transform AttachPoint { get { return m_AttachPoint; } set { m_AttachPoint = value; } }
    }
}