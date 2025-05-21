using ProtoBuf;
using System;

namespace ZHSM
{
    [Serializable, ProtoContract(Name = @"BrigeHeartbeatResponse")]
    public class BrigeHeartbeatResponse : SCPacketBase
    {
        public override int Id
        {
            get
            {
                return 601;
            }
        }
        
        [ProtoMember(1)]
        public int FrameCount;

        public override void Clear()
        {
            FrameCount = 0;
        }
    }
}
