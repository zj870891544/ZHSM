using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace ZHSM
{
    [System.Serializable]
    public class TeamConfig
    {
        [Title("Player")]
        public string playerName;
        public float playerHeight;
        
        [Title("Network")]
        public bool isHost;
        public string host;
        public ushort port;
    }
}