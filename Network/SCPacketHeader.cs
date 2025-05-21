namespace ZHSM
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

        public override int Id { get; set; }
        public override int PacketLength { get; set; }
        public override int Version { get; set; }
        public override int ExCode { get; set; }
    }
}
