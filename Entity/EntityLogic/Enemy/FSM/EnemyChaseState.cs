using GameFramework.Fsm;
using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.Enemy.Conditions;

namespace ZHSM.Enemy
{
    public class EnemyChaseState : EnemyState
    {
        protected override void OnInit(IFsm<EnemyEntity> fsm)
        {
            base.OnInit(fsm);

            // attack target
            FsmTransition attackTransition = new FsmTransition(typeof(EnemyAttackState));
            attackTransition.AddCondition(new AttackCondition(m_Enemy));
            AddTransition(attackTransition);
            
            // chase lose target
            FsmTransition loseTransition = new FsmTransition(typeof(EnemyIdleState));
            loseTransition.AddCondition(new ChaseLoseCondition(m_Enemy));
            AddTransition(loseTransition);
        }

        protected override void OnEnter(IFsm<EnemyEntity> fsm)
        {
            base.OnEnter(fsm);
            
            m_Enemy.agent.stoppingDistance = m_Enemy.attackDistance;
            m_Enemy.agent.speed = m_Enemy.followSpeed;
            m_Enemy.agent.isStopped = false;
            
            m_Enemy.animator.SetBool("IsAttack", false);
            m_Enemy.animator.SetBool("IsDead", false);
            m_Enemy.animator.SetFloat("MoveSpeed", 1.0f);
        }

        protected override void OnUpdate(IFsm<EnemyEntity> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);

            if (m_Enemy.closestTarget)
            {
                m_Enemy.agent.SetDestination(m_Enemy.closestTarget.Position);
            }
        }
    }
}