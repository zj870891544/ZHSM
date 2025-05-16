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
            DefenseMultiplier = 0.5f;//假设防御状态下，伤害减少50%
        }

        /// <summary>
        /// 是否处于防御状态
        /// </summary>
        public bool IsDefending { get;  set; }

        /// <summary>
        /// 防御状态下伤害减免倍率(0-1之间，0代表完全免疫伤害，1代表无伤害减免)
        /// </summary>
        public float DefenseMultiplier { get; private set; }

    }
}


