using ProtoBuf;

namespace ZHSM.LogicServer
{
    public sealed class SCPacketHeader : PacketHeaderBase
    {
        public override PacketType PacketType
        {
            get
            {
                return PacketType.ServerToClient;
            }
        }

        [ProtoMember(1)]
        public override int Id { get; set; }
        [ProtoMember(2)]
        public override int PacketLength { get; set; }
    }
}
