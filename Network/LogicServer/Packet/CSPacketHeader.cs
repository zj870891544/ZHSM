using System;
using ProtoBuf;

namespace ZHSM.LogicServer
{
    [Serializable, ProtoContract(Name = @"CSPacketHeader")]
    public class CSPacketHeader : PacketHeaderBase
    {
        public override PacketType PacketType
        {
            get
            {
                return PacketType.ClientToServer;
            }
        }

        [ProtoMember(1)]
        public override int Id { get; set; }
        [ProtoMember(2)]
        public override int PacketLength { get; set; }
    }
}
