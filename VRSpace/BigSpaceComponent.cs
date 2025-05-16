using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class BigSpaceComponent : GameFrameworkComponent
    {
        [SerializeField] private TeamConfig teamConfig;
        
        [Title("初始配置")] [PropertySpace]
        [SerializeField] private int characterId;
        [SerializeField] private int weaponId;

        public int CharacterId
        {
            get => characterId;
            set => characterId = value;
        }

        public int WeaponId
        {
            get => weaponId;
            set => weaponId = value;
        }

        public string playerName => teamConfig.playerName;
        public float playerHeight => teamConfig.playerHeight;
        public bool isHost => teamConfig.isHost;
        public string Host => teamConfig.host;
        public ushort Port => teamConfig.port;
        
        public void SetTeamConfig(TeamConfig config)
        {
            teamConfig = config;
        }
    }
}