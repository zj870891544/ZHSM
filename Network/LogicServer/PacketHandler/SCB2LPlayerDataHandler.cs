using GameFramework.Network;
using UnityGameFramework.Runtime;

namespace ZHSM.LogicServer
{
    public class SCB2LPlayerDataHandler : PacketHandlerBase
    {
        public override int Id => 102;
        
        public override void Handle(object sender, Packet packet)
        {
            SCB2LPlayerData b2lPlayerData = (SCB2LPlayerData)packet;
            if (b2lPlayerData == null) return;
            
            GameEntry.BigSpace.UpdateB2LPlayers(b2lPlayerData);
        }
    }
}