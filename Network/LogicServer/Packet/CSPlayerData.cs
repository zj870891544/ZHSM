using System;
using GameFramework;
using ProtoBuf;

namespace ZHSM.LogicServer
{
    [Serializable, ProtoContract(Name = "CSPlayerData")]
    public class CSPlayerData : CSPacketBase
    {
        public override int Id => 100;
        
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
        public string CurStage;
        [ProtoMember(7)]
        public string CurArea;
        [ProtoMember(8)]
        public string IPAddress;
        
        [ProtoMember(100)]
        public string ContentID;
        [ProtoMember(101)]
        public string ContentName;
        [ProtoMember(102)]
        public string TeamID;
        [ProtoMember(103)]
        public string TeamName;
        [ProtoMember(104)]
        public string JsonData;
        
        public override void Clear()
        {
            ID = string.Empty;
            PrefabName = string.Empty;
            PlayerNick = string.Empty;
            ContentID = string.Empty;
            TeamID = string.Empty;
            TeamName = string.Empty;
            CurStage = string.Empty;
            CurArea = string.Empty;
            JsonData = string.Empty;
            
            ReferencePool.Release(Pos);
            ReferencePool.Release(Rotation);
        }
    }
}