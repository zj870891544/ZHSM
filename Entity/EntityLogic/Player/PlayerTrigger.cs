using UnityEngine;

namespace ZHSM
{
    public class PlayerTrigger : MonoBehaviour
    {
        private NetworkPlayer m_NetworkPlayer;
        
        public uint NetId => m_NetworkPlayer.netId;

        private void Start()
        {
            m_NetworkPlayer = GetComponentInParent<NetworkPlayer>();
        }
    }
}