using GameFramework.Fsm;
using UnityEngine;
using ZHSM.Enemy.Conditions;

namespace ZHSM.Enemy
{
    public class EnemyPatrolState : EnemyState
    {
        private const float PATROL_MIN_DISTANCE = 0.1f;//巡逻到达目标最近距离
        
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

            // patrol
            FsmTransition patrolTransition = new FsmTransition(typeof(EnemyIdleState));
            patrolTransition.AddCondition(new PatrolCondition(m_Enemy, PATROL_MIN_DISTANCE));
            AddTransition(patrolTransition);
        }

        protected override void OnEnter(IFsm<EnemyEntity> fsm)
        {
            base.OnEnter(fsm);
            
            m_Enemy.agent.speed = m_Enemy.patrolSpeed;
            m_Enemy.agent.isStopped = false;
            m_Enemy.agent.stoppingDistance = 0.0f;
            m_Enemy.agent.SetDestination(m_Enemy.PatrolPosition);
            
            m_Enemy.animator.SetBool("IsAttack", false);
            m_Enemy.animator.SetBool("IsDead", false);
            m_Enemy.animator.SetFloat("MoveSpeed",
                Mathf.Clamp(m_Enemy.patrolSpeed, 0, m_Enemy.followSpeed) / m_Enemy.followSpeed);
        }
    }
}