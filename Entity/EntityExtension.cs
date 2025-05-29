using System;
using Mirror;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public static class EntityExtension
    {
        // 关于 EntityId 的约定：
        // 0 为无效
        // 正值用于和服务器通信的实体（如玩家角色、NPC、怪等，服务器只产生正值）
        // 负值用于本地生成的临时实体（如特效、FakeObject等）
        private static int s_SerialId = 0;

        public static Entity GetGameEntity(this EntityComponent entityComponent, int entityId)
        {
            UnityGameFramework.Runtime.Entity entity = entityComponent.GetEntity(entityId);
            if (entity == null)
            {
                return null;
            }

            return (Entity)entity.Logic;
        }

        public static void HideEntity(this EntityComponent entityComponent, Entity entity)
        {
            entityComponent.HideEntity(entity.Entity);
        }

        public static void AttachEntity(this EntityComponent entityComponent, Entity entity, int ownerId, string parentTransformPath = null, object userData = null)
        {
            entityComponent.AttachEntity(entity.Entity, ownerId, parentTransformPath, userData);
        }
        
        public static void ShowEffect(this EntityComponent entityComponent, EffectData data)
        {
            entityComponent.ShowEntity(typeof(EffectEntity), "Effect", Constant.AssetPriority.EffectAsset, data);
        }
        
        public static void ShowBullet(this EntityComponent entityComponent, BulletData bulletData)
        {
            entityComponent.ShowEntity(typeof(BulletEntity), "Bullet", Constant.AssetPriority.BulletAsset, bulletData);
        }

        public static void ShowPlayer(this EntityComponent entityComponent, CharacterData data)
        {
            entityComponent.ShowEntity(typeof(PlayerEntity), "Character", Constant.AssetPriority.CharacterAsset, data);
        }

        public static void ShowB2LPlayer(this EntityComponent entityComponent, B2LPlayerEntityData data)
        {
            entityComponent.ShowEntity(typeof(B2LPlayerEntity), "Character", Constant.AssetPriority.CharacterAsset, data);
        }

        public static void ShowWeapon(this EntityComponent entityComponent, WeaponData weaponData)
        {
            entityComponent.ShowEntity(typeof(WeaponEntity), "Weapon", Constant.AssetPriority.WeaponAsset, weaponData);
        }

        public static void ShowEnemy(this EntityComponent entityComponent, EnemyData enemyData)
        {
            entityComponent.ShowEntity(typeof(Enemy.EnemyEntity), "Enemy", Constant.AssetPriority.EnemyAsset,
                enemyData);
        }

        public static void ShowBoss(this EntityComponent entityComponent, BossData bossData)
        {
            entityComponent.ShowEntity(typeof(BossEntity), "Enemy", Constant.AssetPriority.EnemyAsset, bossData);
        }
        
        public static void ShowBossProjectile(this EntityComponent entityComponent, BossProjectileData projectileData)
        {
            entityComponent.ShowEntity(typeof(BossProjectile), "Bullet", Constant.AssetPriority.BulletAsset, projectileData);
        }

        public static void ShowDefenseTower(this EntityComponent entityComponent, DefenseTowerData towerData)
        {
            entityComponent.ShowEntity(typeof(DefenseTower), "DefenseTower", Constant.AssetPriority.BuildingAsset, towerData);
        }

        public static void ShowCircleIndicator(this EntityComponent entityComponent, CircleIndicatorData circleIndicatorData)
        {
            entityComponent.ShowEntity(typeof(CircleIndicator), "Effect", Constant.AssetPriority.EffectAsset, circleIndicatorData);
        }

        private static void ShowEntity(this EntityComponent entityComponent, Type logicType, string entityGroup, int priority, EntityData data)
        {
            if (data == null)
            {
                Log.Warning("Data is invalid.");
                return;
            }

            if (GameEntry.LubanTable.TryGetTables(out cfg.Tables tables))
            {
                var tbEntity = tables.TbEntityCfg.Get(data.TypeId);
                if (tbEntity == null)
                {
                    Log.Warning("Can not load entity id '{0}' from data table.", data.TypeId.ToString());
                    return;
                }
                
                entityComponent.ShowEntity(data.Id, logicType, AssetUtility.GetEntityAsset(tbEntity.AssetName), entityGroup, priority, data);
            }
        }

        public static int GenerateSerialId(this EntityComponent entityComponent)
        {
            return --s_SerialId;
        }
    }
}