using GameFramework;
using GameFramework.Network;
using UnityGameFramework.Runtime;

namespace ZHSM.LogicServer
{
    public class SCHeartBeatHandler : PacketHandlerBase
    {
        public override int Id => 201;

        public override void Handle(object sender, Packet packet)
        {
            SCHeartbeat packetImpl = (SCHeartbeat)packet;
            // Log.Info($"[Network] 收到心跳  {packetImpl.FrameCount}");
            
        }
    }
}
