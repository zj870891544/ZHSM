using ProtoBuf;
using System;

namespace ZHSM.LogicServer
{
    [Serializable, ProtoContract(Name = @"SCHeartbeat")]
    public class SCHeartbeat : SCPacketBase
    {
        public override int Id => 201;
        
        [ProtoMember(1)]
        public int FrameCount;

        public override void Clear()
        {
            FrameCount = 0;
        }
    }
}
