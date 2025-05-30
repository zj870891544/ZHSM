
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
public sealed partial class EnemyCfg : Luban.BeanBase
{
    public EnemyCfg(JSONNode _buf) 
    {
        { if(!_buf["Id"].IsNumber) { throw new SerializationException(); }  Id = _buf["Id"]; }
        { if(!_buf["EntityId"].IsNumber) { throw new SerializationException(); }  EntityId = _buf["EntityId"]; }
        { if(!_buf["BulletId"].IsNumber) { throw new SerializationException(); }  BulletId = _buf["BulletId"]; }
        { if(!_buf["FirePoint"].IsString) { throw new SerializationException(); }  FirePoint = _buf["FirePoint"]; }
        { if(!_buf["Type"].IsNumber) { throw new SerializationException(); }  Type = (EnemyType)_buf["Type"].AsInt; }
        { var __json0 = _buf["TargetCamps"]; if(!__json0.IsArray) { throw new SerializationException(); } TargetCamps = new System.Collections.Generic.List<int>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  TargetCamps.Add(__v0); }   }
        { if(!_buf["FlashEffectId"].IsNumber) { throw new SerializationException(); }  FlashEffectId = _buf["FlashEffectId"]; }
        { if(!_buf["HitEffectId"].IsNumber) { throw new SerializationException(); }  HitEffectId = _buf["HitEffectId"]; }
        { var __json0 = _buf["FireSounds"]; if(!__json0.IsArray) { throw new SerializationException(); } int _n0 = __json0.Count; FireSounds = new int[_n0]; int __index0=0; foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  FireSounds[__index0++] = __v0; }   }
        { var __json0 = _buf["HitSounds"]; if(!__json0.IsArray) { throw new SerializationException(); } int _n0 = __json0.Count; HitSounds = new int[_n0]; int __index0=0; foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  HitSounds[__index0++] = __v0; }   }
        { var __json0 = _buf["DamageSounds"]; if(!__json0.IsArray) { throw new SerializationException(); } int _n0 = __json0.Count; DamageSounds = new int[_n0]; int __index0=0; foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  DamageSounds[__index0++] = __v0; }   }
        { var __json0 = _buf["FoodStepSounds"]; if(!__json0.IsArray) { throw new SerializationException(); } int _n0 = __json0.Count; FoodStepSounds = new int[_n0]; int __index0=0; foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  FoodStepSounds[__index0++] = __v0; }   }
        { var __json0 = _buf["DeadSounds"]; if(!__json0.IsArray) { throw new SerializationException(); } int _n0 = __json0.Count; DeadSounds = new int[_n0]; int __index0=0; foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  DeadSounds[__index0++] = __v0; }   }
        { if(!_buf["PatrolSpeed"].IsNumber) { throw new SerializationException(); }  PatrolSpeed = _buf["PatrolSpeed"]; }
        { if(!_buf["FollowDistance"].IsNumber) { throw new SerializationException(); }  FollowDistance = _buf["FollowDistance"]; }
        { if(!_buf["FollowSpeed"].IsNumber) { throw new SerializationException(); }  FollowSpeed = _buf["FollowSpeed"]; }
        { if(!_buf["AttackDistance"].IsNumber) { throw new SerializationException(); }  AttackDistance = _buf["AttackDistance"]; }
        { if(!_buf["HP"].IsNumber) { throw new SerializationException(); }  HP = _buf["HP"]; }
        { if(!_buf["Damage"].IsNumber) { throw new SerializationException(); }  Damage = _buf["Damage"]; }
        { if(!_buf["Mass"].IsNumber) { throw new SerializationException(); }  Mass = _buf["Mass"]; }
    }

    public static EnemyCfg DeserializeEnemyCfg(JSONNode _buf)
    {
        return new EnemyCfg(_buf);
    }

    /// <summary>
    /// 小怪编号
    /// </summary>
    public readonly int Id;
    /// <summary>
    /// 实体ID
    /// </summary>
    public readonly int EntityId;
    /// <summary>
    /// 子弹实体ID
    /// </summary>
    public readonly int BulletId;
    /// <summary>
    /// 发射点路径
    /// </summary>
    public readonly string FirePoint;
    /// <summary>
    /// 小怪类型
    /// </summary>
    public readonly EnemyType Type;
    /// <summary>
    /// 目标阵营列表
    /// </summary>
    public readonly System.Collections.Generic.List<int> TargetCamps;
    /// <summary>
    /// 枪火特效ID
    /// </summary>
    public readonly int FlashEffectId;
    /// <summary>
    /// 子弹碰撞特效ID
    /// </summary>
    public readonly int HitEffectId;
    /// <summary>
    /// 开火音效
    /// </summary>
    public readonly int[] FireSounds;
    /// <summary>
    /// 子弹碰撞音效
    /// </summary>
    public readonly int[] HitSounds;
    /// <summary>
    /// 受击音效
    /// </summary>
    public readonly int[] DamageSounds;
    /// <summary>
    /// 脚步音效
    /// </summary>
    public readonly int[] FoodStepSounds;
    /// <summary>
    /// 死亡音效
    /// </summary>
    public readonly int[] DeadSounds;
    /// <summary>
    /// 巡逻速度
    /// </summary>
    public readonly float PatrolSpeed;
    /// <summary>
    /// 追击距离
    /// </summary>
    public readonly float FollowDistance;
    /// <summary>
    /// 追击速度
    /// </summary>
    public readonly float FollowSpeed;
    /// <summary>
    /// 攻击距离
    /// </summary>
    public readonly float AttackDistance;
    /// <summary>
    /// 最大血量
    /// </summary>
    public readonly int HP;
    /// <summary>
    /// 基础伤害
    /// </summary>
    public readonly int Damage;
    /// <summary>
    /// 体量
    /// </summary>
    public readonly float Mass;
   
    public const int __ID__ = 1831866332;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "EntityId:" + EntityId + ","
        + "BulletId:" + BulletId + ","
        + "FirePoint:" + FirePoint + ","
        + "Type:" + Type + ","
        + "TargetCamps:" + Luban.StringUtil.CollectionToString(TargetCamps) + ","
        + "FlashEffectId:" + FlashEffectId + ","
        + "HitEffectId:" + HitEffectId + ","
        + "FireSounds:" + Luban.StringUtil.CollectionToString(FireSounds) + ","
        + "HitSounds:" + Luban.StringUtil.CollectionToString(HitSounds) + ","
        + "DamageSounds:" + Luban.StringUtil.CollectionToString(DamageSounds) + ","
        + "FoodStepSounds:" + Luban.StringUtil.CollectionToString(FoodStepSounds) + ","
        + "DeadSounds:" + Luban.StringUtil.CollectionToString(DeadSounds) + ","
        + "PatrolSpeed:" + PatrolSpeed + ","
        + "FollowDistance:" + FollowDistance + ","
        + "FollowSpeed:" + FollowSpeed + ","
        + "AttackDistance:" + AttackDistance + ","
        + "HP:" + HP + ","
        + "Damage:" + Damage + ","
        + "Mass:" + Mass + ","
        + "}";
    }
}

}

