using System;
using ProtoBuf;

namespace ZHSM.LogicServer
{
    [Serializable, ProtoContract(Name = "SyncPlayerData")]
    public class SyncPlayerData
    {
        [ProtoMember(1)]
        public string ID;
        [ProtoMember(2)]
        public string PrefabName;
        [ProtoMember(3)]
        public string PlayerNick;
        [ProtoMember(4)]
        public FVector3 Pos;
        [ProtoMember(5)]
        public FVector4 Rotation;
        [ProtoMember(6)]
        public string ContentID;
        [ProtoMember(7)]
        public string TeamID;
        [ProtoMember(8)]
        public string TeamName;
        [ProtoMember(9)]
        public string JsonData;
    }
}