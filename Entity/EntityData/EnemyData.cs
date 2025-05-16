using UnityEngine;
using ZHSM.cfg;

namespace ZHSM
{
    public class EnemyData : TargetableObjectData
    {
        private EnemyCfg m_EnemyCfg;
        private int m_MaxHP;
        
        public EnemyData(int entityId, int typeId, CampType camp, int enemyId) : base(entityId, typeId, camp)
        {
            TbEnemyCfg tbEnemyCfg = GameEntry.LubanTable.GetTbEnemyCfg();
            m_EnemyCfg = tbEnemyCfg.GetOrDefault(enemyId);

            m_MaxHP = HP = m_EnemyCfg.HP;
        }

        public override int MaxHP => m_MaxHP;
        public EnemyCfg EnemyCfg => m_EnemyCfg;
        public int EnemyId => m_EnemyCfg.Id;

        //=========================== 巡逻相关
        public bool PatrolEnable;
        public Vector3 PatrolCenter;
        public float PatrolRadius;
    }
}