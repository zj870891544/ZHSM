using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.LogicServer;

namespace ZHSM
{
    public class B2LPlayerEntity : Entity
    {
        
        private B2LPlayerEntityData m_Data = null;
        private Vector3 m_Position;
        private Quaternion m_Rotation;
        
        [SerializeField] private float m_DelayTimer;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            m_Data = userData as B2LPlayerEntityData;
            if (m_Data == null)
            {
                Log.Error("B2LPlayerEntitData is invalid.");
                return;
            }

            m_DelayTimer = 0.0f;
            
            GameEntry.BigSpace.AddB2LPlayerEntity(m_Data.PlayerId, this);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            
            CachedTransform.SetPositionAndRotation(m_Position, m_Rotation);

            m_DelayTimer += elapseSeconds;
        }

        public bool CheckVisible()
        {
            return m_DelayTimer < m_Data.RemoveDeley;
        }

        public void UpdatePositionRotation(FVector3 pos, FVector4 rot)
        {
            m_DelayTimer = 0.0f;
            
            m_Position.x = pos.x;
            m_Position.y = pos.y;
            m_Position.z = pos.z;
            
            m_Rotation.x = rot.x;
            m_Rotation.y = rot.y;
            m_Rotation.z = rot.z;
            m_Rotation.w = rot.w;
        }
    }
}