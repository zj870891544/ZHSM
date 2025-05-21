using Mirror;
using UnityEngine;

namespace ZHSM
{
    /// <summary>
    /// 盾牌数据类
    /// </summary>
    public class ShieldData : WeaponData
    {
        public ShieldData(int entityId, int typeId,int shieldId,NetworkConnectionToClient connection) : base(entityId, typeId, shieldId, connection)
        {
            IsDefending = false;
            DefenseMultiplier = 0.5f;//处于防御状态下，伤害降低50%
        }

        /// <summary>
        /// 是否处于防御状态
        /// </summary>
        public bool IsDefending { get;  set; }

        /// <summary>
        /// 防御状态下伤害减免倍率(0-1之间，0表示完全阻挡伤害，1表示伤害不减)
        /// </summary>
        public float DefenseMultiplier { get; private set; }

    }
}


