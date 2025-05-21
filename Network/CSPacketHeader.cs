using System;
using ProtoBuf;

namespace ZHSM
{
    [Serializable, ProtoContract(Name = @"CSPacketHeader")]
    public sealed class CSPacketHeader : PacketHeaderBase
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
        [ProtoMember(3)]
        public override int Version { get; set; }
        [ProtoMember(4)]
        public override int ExCode { get; set; }
    }
}
