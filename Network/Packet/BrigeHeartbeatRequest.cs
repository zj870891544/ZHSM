using ProtoBuf;
using System;

namespace ZHSM
{
    [Serializable, ProtoContract(Name = @"BrigeHeartbeatRequest")]
    public class BrigeHeartbeatRequest : CSPacketBase
    {
        public override int Id
        {
            get
            {
                return 600;
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
