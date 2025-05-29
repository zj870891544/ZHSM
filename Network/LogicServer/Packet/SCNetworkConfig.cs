using System;
using ProtoBuf;

namespace ZHSM.LogicServer
{
    [Serializable, ProtoContract(Name = nameof(SCNetworkConfig))]
    public class SCNetworkConfig : SCPacketBase
    {
        public override int Id => 202;

        [ProtoMember(1)]
        public bool IsHost;
        [ProtoMember(2)]
        public string ServerIP;
        [ProtoMember(3)]
        public int ServerPort;
        
        public override void Clear()
        {
            IsHost = false;
            ServerIP = null;
            ServerPort = 0;
        }
    }
}