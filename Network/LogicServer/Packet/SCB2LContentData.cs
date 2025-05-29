using System;
using System.Collections.Generic;
using ProtoBuf;

namespace ZHSM.LogicServer
{
    [Serializable, ProtoContract(Name = "B2LPlayerData")]
    public class SCB2LPlayerData : SCPacketBase
    {
        public override int Id => 102;
        
        [ProtoMember(1)]
        public string ID;
        [ProtoMember(2)]
        public string ContentID;
        [ProtoMember(3)]
        public List<SyncPlayerData> CollisionPlayers;

        public override void Clear()
        {
            ID = null;
            ContentID = null;
            CollisionPlayers = null;
        }
    }
}