
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using SimpleJSON;


namespace ZHSM.cfg
{
public sealed partial class Rotation : Luban.BeanBase
{
    public Rotation(JSONNode _buf) 
    {
        { if(!_buf["x"].IsNumber) { throw new SerializationException(); }  X = _buf["x"]; }
        { if(!_buf["y"].IsNumber) { throw new SerializationException(); }  Y = _buf["y"]; }
        { if(!_buf["z"].IsNumber) { throw new SerializationException(); }  Z = _buf["z"]; }
        { if(!_buf["w"].IsNumber) { throw new SerializationException(); }  W = _buf["w"]; }
    }

    public static Rotation DeserializeRotation(JSONNode _buf)
    {
        return new Rotation(_buf);
    }

    public readonly float X;
    public readonly float Y;
    public readonly float Z;
    public readonly float W;
   
    public const int __ID__ = 24343454;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "x:" + X + ","
        + "y:" + Y + ","
        + "z:" + Z + ","
        + "w:" + W + ","
        + "}";
    }
}

}

