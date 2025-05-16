using GameFramework.Fsm;
using UnityEngine;
using ZHSM.Enemy.Conditions;

namespace ZHSM.Enemy
{
    public class EnemyIdleState : EnemyState
    {
        protected override void OnInit(IFsm<EnemyEntity> fsm)
        {
            base.OnInit(fsm);

            // attack target
            FsmTransition attackTransition = new FsmTransition(typeof(EnemyAttackState));
            attackTransition.AddCondition(new AttackCondition(m_Enemy));
            AddTransition(attackTransition);
            
            // chase target
            FsmTransition followTransition = new FsmTransition(typeof(EnemyChaseState));
            followTransition.AddCondition(new ChaseCondition(m_Enemy));
            AddTransition(followTransition);

            // wait random timer
            if (m_Enemy.PatrolEnable)
            {
                FsmTransition waitTransition = new FsmTransition(typeof(EnemyPatrolState));
                waitTransition.AddCondition(new WaitRandomTimeCondition(m_Enemy, 6.0f, 9.0f));
                AddTransition(waitTransition);
            }
        }

        protected override void OnEnter(IFsm<EnemyEntity> fsm)
        {
            base.OnEnter(fsm);

            m_Enemy.agent.isStopped = true;
            m_Enemy.agent.velocity = Vector3.zero;
            m_Enemy.agent.stoppingDistance = 0.0f;
            
            m_Enemy.animator.SetBool("IsAttack", false);
            m_Enemy.animator.SetBool("IsDead", false);
            m_Enemy.animator.SetFloat("MoveSpeed", 0.0f);
        }
    }
}