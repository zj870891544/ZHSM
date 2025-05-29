using GameFramework;
using GameFramework.Event;
using Mirror;
using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.cfg;

namespace ZHSM
{
    /// <summary>
    /// 武器管理组件
    /// </summary>
    public class WeaponManagerComponent : GameFrameworkComponent
    {
        [SerializeField] private int weaponId;
        [SerializeField] private int weaponGroupId;
        
        private TbWeaponGroupCfg m_TbWeaponGroupCfg;
        
        public int WeaponId
        {
            get => weaponId;
            set => weaponId = value;
        }
        
        public int WeaponGroupId
        {
            get => weaponGroupId;
            set => weaponGroupId = value;
        }

        protected override void Awake()
        {
            base.Awake();
            Debug.Log("武器管理组件启动...");
        }

        /// <summary>
        /// 装备指定ID的武器套装
        /// </summary>
        /// <param name="groupId">武器套装ID</param>
        /// <param name="playerObj">玩家对象</param>
        /// <param name="connection">网络连接</param>
        public void EquipWeaponGroup(int groupId, NetworkConnectionToClient connection = null)
        {
            m_TbWeaponGroupCfg = GameEntry.LubanTable.GetTbWeaponGroupCfg();
            
            if (m_TbWeaponGroupCfg == null)
            {
                Log.Error("武器套装配置表未加载!");
                return;
            }

            WeaponGroupCfg weaponGroup = m_TbWeaponGroupCfg.GetOrDefault(groupId);
            if (weaponGroup == null)
            {
                Log.Warning($"未找到ID为{groupId}的武器套装配置!");
                return;
            }

            // 装备左手武器(盾牌)
            if (weaponGroup.LeftWeaponId > 0)
            {
                GameEntry.Entity.ShowWeapon(new WeaponData(GameEntry.Entity.GenerateSerialId(), 
                    GetEntityIdByWeaponId(weaponGroup.LeftWeaponId), 
                    weaponGroup.LeftWeaponId, 
                    connection)
                {
                    Position = Vector3.zero,
                    Rotation = Quaternion.identity
                });
                
                Debug.Log($"已装备左手武器 ID: {weaponGroup.LeftWeaponId}");
            }

            // 装备右手武器(手枪)
            if (weaponGroup.RightWeaponId > 0)
            {
                GameEntry.Entity.ShowWeapon(new WeaponData(GameEntry.Entity.GenerateSerialId(), 
                    GetEntityIdByWeaponId(weaponGroup.RightWeaponId), 
                    weaponGroup.RightWeaponId, 
                    connection)
                {
                    Position = Vector3.zero,
                    Rotation = Quaternion.identity
                });
                
                Debug.Log($"已装备右手武器 ID: {weaponGroup.RightWeaponId}");
            }
            
            Debug.Log($"已成功装备武器套装 ID: {groupId}");
        }

        /// <summary>
        /// 根据武器ID获取对应的实体ID
        /// </summary>
        private int GetEntityIdByWeaponId(int weaponId)
        {
            // 从武器配置表中获取实体ID
            TbWeaponCfg tbWeaponCfg = GameEntry.LubanTable.GetTbWeaponCfg();
            if (tbWeaponCfg != null)
            {
                WeaponCfg weaponCfg = tbWeaponCfg.GetOrDefault(weaponId);
                if (weaponCfg != null)
                {
                    return weaponCfg.EntityId;
                }
            }
            
            Log.Error($"未找到武器ID {weaponId} 对应的实体ID");
            return 0;
        }
    }
} 