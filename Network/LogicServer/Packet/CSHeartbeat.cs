using ProtoBuf;
using System;

namespace ZHSM.LogicServer
{
    [Serializable, ProtoContract(Name = @"CSHeartbeat")]
    public class CSHeartbeat : CSPacketBase
    {
        public override int Id => 200;
        
        [ProtoMember(1)]
        public int FrameCount;

        public override void Clear()
        {
            FrameCount = 0;
        }
    }
}
