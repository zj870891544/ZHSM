using Mirror;
using UnityEngine;

namespace ZHSM
{
    public class NetworkDefenseTower : NetworkTargetable
    {
        [SyncVar] [SerializeField] private bool m_IsOccupied = false;
        [SyncVar] [SerializeField] private float m_OccupyProgress = 0.0f;
        
        public override Vector3 Position => transform.position;

        public void SetOccupyInfo(bool isOccupied, float occupyProgress)
        {
            m_IsOccupied = isOccupied;
            m_OccupyProgress = occupyProgress;
        }
    }
}