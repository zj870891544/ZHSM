using GameFramework;
using GameFramework.Network;
using UnityGameFramework.Runtime;

namespace ZHSM.LogicServer
{
    public class SCNetworkConfigHandler : PacketHandlerBase
    {
        public override int Id => 202;
        
        public override void Handle(object sender, Packet packet)
        {
            SCNetworkConfig networkConfig = packet as SCNetworkConfig;
            if (networkConfig == null) return;

            Log.Info($"获取网络配置 host:{networkConfig.IsHost} ip:{networkConfig.ServerIP} port:{networkConfig.ServerPort}");
            
            GameEntry.Event.Fire(this,
                StartMultiPlayerEventArgs.Create(networkConfig.IsHost, networkConfig.ServerIP,
                    networkConfig.ServerPort));
        }
    }
}