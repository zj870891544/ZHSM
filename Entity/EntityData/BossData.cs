using System.Collections.Generic;
using ZHSM.cfg;

namespace ZHSM
{
    public class BossData : TargetableObjectData
    {
        private EnemyCfg m_EnemyCfg;
        private int m_MaxHP;
        private List<SkillStep> m_SkillSteps;
        private List<LevelWaveConfig> m_WaveConfigs;
        private SkillAoeCircle m_AoeCircleConfig;
        private SkillAoeSector m_AoeSectorConfig;
        private SkillSniper m_SniperConfig;
        
        public BossData(int entityId, int typeId, CampType camp, int enemyId) : base(entityId, typeId, camp)
        {
            TbEnemyCfg tbEnemyCfg = GameEntry.LubanTable.GetTbEnemyCfg();
            m_EnemyCfg = tbEnemyCfg.GetOrDefault(enemyId);
            
            m_MaxHP = HP = m_EnemyCfg.HP;
        }

        public override int MaxHP => m_MaxHP;
        public List<SkillStep> SkillSteps { get => m_SkillSteps; set => m_SkillSteps = value; }
        public List<LevelWaveConfig> WaveConfigs { get => m_WaveConfigs; set => m_WaveConfigs = value; }
        public SkillAoeCircle AoeCircleConfig { get => m_AoeCircleConfig; set => m_AoeCircleConfig = value; }
        public SkillAoeSector AoeSectorConfig { get => m_AoeSectorConfig; set => m_AoeSectorConfig = value; }
        public SkillSniper SniperConfig { get => m_SniperConfig; set => m_SniperConfig = value; }
    }
}