using GameFramework.Fsm;
using UnityGameFramework.Runtime;

namespace ZHSM.FSM
{
    public class BossState : FsmState<BossEntity>
    {
        private IFsm<BossEntity> m_FSM;
        protected BossEntity m_Boss;
        
        protected override void OnInit(IFsm<BossEntity> fsm)
        {
            base.OnInit(fsm);

            m_FSM = fsm;
            m_Boss = fsm.Owner;
        }

        /// <summary>
        /// 技能完成
        /// </summary>
        protected void OnSkillComplete()
        {
            BossSkillType skillType = m_Boss.GetSkillType();
            switch (skillType)
            {
                case BossSkillType.SpawnEnemy:
                    ChangeState<BossSummonState>(m_FSM);
                    break;
                case BossSkillType.AoeCircle:
                    ChangeState<BossAoeCircleState>(m_FSM);
                    break;
                case BossSkillType.Sniper:
                    ChangeState<BossSniperState>(m_FSM);
                    break;
                case BossSkillType.AoeSector:
                    ChangeState<BossAoeSectorState>(m_FSM);
                    break;
                default:
                    Log.Error($"Unknow skill type: {skillType}");
                    break;
            }
        }
    }
}