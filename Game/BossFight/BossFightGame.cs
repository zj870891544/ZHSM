using GameFramework.Event;
using UnityGameFramework.Runtime;
using ZHSM.cfg;

namespace ZHSM
{
    public class BossFightGame : GameBase
    {
        public override GameMode GameMode => GameMode.BossFight;

        private TbEnemyCfg m_TbEnemyCfg;
        private BossFightConfig m_LevelConfig;
        private int m_BossEntityId;
        private BossEntity m_BossEntity;

        protected override void OnInitializeSuccess(LevelConfig levelConfig)
        {
            base.OnInitializeSuccess(levelConfig);
            
            m_LevelConfig = levelConfig as BossFightConfig;
            if (m_LevelConfig == null)
            {
                Log.Error($"Load BossFightConfig parse error: {m_LevelConfig.name}");
                return;
            }

            m_TbEnemyCfg = GameEntry.LubanTable.GetTbEnemyCfg();
            
            CreateBoss();
        }

        private void CreateBoss()
        {
            EnemyCfg enemyCfg = tbEnemyCfg.GetOrDefault(m_LevelConfig.bossId);
            if (enemyCfg == null)
            {
                Log.Error($"boss {m_LevelConfig.bossId} not found in TbEnemyCfg.");
                return;
            }

            m_BossEntityId = GameEntry.Entity.GenerateSerialId();
            GameEntry.Entity.ShowBoss(new BossData(m_BossEntityId, enemyCfg.EntityId, CampType.Enemy, enemyCfg.Id)
            {
                Position = m_LevelConfig.bossPosition,
                Rotation = m_LevelConfig.bossRotation,
                SkillSteps = m_LevelConfig.skillSteps,
                WaveConfigs = m_LevelConfig.waves,
                AoeCircleConfig = m_LevelConfig.aoeCircle,
                SniperConfig = m_LevelConfig.sniper,
                AoeSectorConfig = m_LevelConfig.aoeSector
            });
        }

        protected override void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = e as ShowEntitySuccessEventArgs;
            if(ne == null) return;

            if (m_BossEntityId == ne.Entity.Id)
            {
                m_BossEntity = ne.Entity.Logic as BossEntity;
                
                Log.Info($"boss {m_LevelConfig.bossId} 加载成功...");
            }
        }
    }
}