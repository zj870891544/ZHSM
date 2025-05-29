using System;
using GameFramework;
using ProtoBuf;

namespace ZHSM.LogicServer
{
    [Serializable, ProtoContract(Name = "FVector4")]
    public class FVector4 : IReference
    {
        [ProtoMember(1)]
        public float x;
        [ProtoMember(2)]
        public float y;
        [ProtoMember(3)]
        public float z;
        [ProtoMember(4)]
        public float w;

        public void Clear()
        {
            x = y = z = w = 0;
        }
    }
}