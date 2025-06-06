
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
public sealed partial class LevelWave : Luban.BeanBase
{
    public LevelWave(JSONNode _buf) 
    {
        { var __json0 = _buf["spawnPoints"]; if(!__json0.IsArray) { throw new SerializationException(); } SpawnPoints = new System.Collections.Generic.List<int>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  SpawnPoints.Add(__v0); }   }
        { var __json0 = _buf["enemyList"]; if(!__json0.IsArray) { throw new SerializationException(); } EnemyList = new System.Collections.Generic.List<LevelWaveEnemy>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { LevelWaveEnemy __v0;  { if(!__e0.IsObject) { throw new SerializationException(); }  __v0 = LevelWaveEnemy.DeserializeLevelWaveEnemy(__e0);  }  EnemyList.Add(__v0); }   }
        { var _j = _buf["preEvent"]; if (_j.Tag != JSONNodeType.None && _j.Tag != JSONNodeType.NullValue) { { if(!_j.IsString) { throw new SerializationException(); }  PreEvent = _j; } } else { PreEvent = null; } }
        { var _j = _buf["postEvent"]; if (_j.Tag != JSONNodeType.None && _j.Tag != JSONNodeType.NullValue) { { if(!_j.IsString) { throw new SerializationException(); }  PostEvent = _j; } } else { PostEvent = null; } }
    }

    public static LevelWave DeserializeLevelWave(JSONNode _buf)
    {
        return new LevelWave(_buf);
    }

    /// <summary>
    /// 刷怪点索引列表
    /// </summary>
    public readonly System.Collections.Generic.List<int> SpawnPoints;
    /// <summary>
    /// 一波刷怪列表
    /// </summary>
    public readonly System.Collections.Generic.List<LevelWaveEnemy> EnemyList;
    /// <summary>
    /// 前置事件
    /// </summary>
    public readonly string PreEvent;
    /// <summary>
    /// 后置事件
    /// </summary>
    public readonly string PostEvent;
   
    public const int __ID__ = 347824253;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        foreach (var _e in EnemyList) { _e?.ResolveRef(tables); }
    }

    public override string ToString()
    {
        return "{ "
        + "spawnPoints:" + Luban.StringUtil.CollectionToString(SpawnPoints) + ","
        + "enemyList:" + Luban.StringUtil.CollectionToString(EnemyList) + ","
        + "preEvent:" + PreEvent + ","
        + "postEvent:" + PostEvent + ","
        + "}";
    }
}

}

