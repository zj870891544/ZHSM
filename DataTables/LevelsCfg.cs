
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
public sealed partial class LevelsCfg : Luban.BeanBase
{
    public LevelsCfg(JSONNode _buf) 
    {
        { if(!_buf["Id"].IsNumber) { throw new SerializationException(); }  Id = _buf["Id"]; }
        { if(!_buf["GameMode"].IsNumber) { throw new SerializationException(); }  GameMode = _buf["GameMode"]; }
        { if(!_buf["SceneId"].IsNumber) { throw new SerializationException(); }  SceneId = _buf["SceneId"]; }
        { if(!_buf["Rate"].IsNumber) { throw new SerializationException(); }  Rate = _buf["Rate"]; }
        { if(!_buf["IsFinalLevel"].IsBoolean) { throw new SerializationException(); }  IsFinalLevel = _buf["IsFinalLevel"]; }
    }

    public static LevelsCfg DeserializeLevelsCfg(JSONNode _buf)
    {
        return new LevelsCfg(_buf);
    }

    /// <summary>
    /// 编号
    /// </summary>
    public readonly int Id;
    /// <summary>
    /// 游戏模式
    /// </summary>
    public readonly int GameMode;
    /// <summary>
    /// 场景ID
    /// </summary>
    public readonly int SceneId;
    /// <summary>
    /// 刷新频率（秒）
    /// </summary>
    public readonly float Rate;
    /// <summary>
    /// 是否是最终关卡
    /// </summary>
    public readonly bool IsFinalLevel;
   
    public const int __ID__ = 348629077;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "GameMode:" + GameMode + ","
        + "SceneId:" + SceneId + ","
        + "Rate:" + Rate + ","
        + "IsFinalLevel:" + IsFinalLevel + ","
        + "}";
    }
}

}

