using System;
using GameFramework;
using ProtoBuf;

namespace ZHSM.LogicServer
{
    [Serializable, ProtoContract(Name = "FVector3")]
    public class FVector3 : IReference
    {
        [ProtoMember(1)]
        public float x;
        [ProtoMember(2)]
        public float y;
        [ProtoMember(3)]
        public float z;

        public void Clear()
        {
            x = y = z = 0;
        }
    }
}