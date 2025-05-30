
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
public sealed partial class SceneCfg : Luban.BeanBase
{
    public SceneCfg(JSONNode _buf) 
    {
        { if(!_buf["Id"].IsNumber) { throw new SerializationException(); }  Id = _buf["Id"]; }
        { if(!_buf["Name"].IsString) { throw new SerializationException(); }  Name = _buf["Name"]; }
        { if(!_buf["AssetName"].IsString) { throw new SerializationException(); }  AssetName = _buf["AssetName"]; }
        { var __json0 = _buf["BackgroundMusicIds"]; if(!__json0.IsArray) { throw new SerializationException(); } int _n0 = __json0.Count; BackgroundMusicIds = new int[_n0]; int __index0=0; foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  BackgroundMusicIds[__index0++] = __v0; }   }
    }

    public static SceneCfg DeserializeSceneCfg(JSONNode _buf)
    {
        return new SceneCfg(_buf);
    }

    /// <summary>
    /// 实体编号
    /// </summary>
    public readonly int Id;
    /// <summary>
    /// 名称
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// 场景资源名称
    /// </summary>
    public readonly string AssetName;
    /// <summary>
    /// 背景音乐编号
    /// </summary>
    public readonly int[] BackgroundMusicIds;
   
    public const int __ID__ = -710870952;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "Name:" + Name + ","
        + "AssetName:" + AssetName + ","
        + "BackgroundMusicIds:" + Luban.StringUtil.CollectionToString(BackgroundMusicIds) + ","
        + "}";
    }
}

}

