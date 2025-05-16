using GameFramework.Fsm;
using UnityEngine;
using ZHSM.Enemy.Conditions;

namespace ZHSM.Enemy
{
    public class EnemyAttackState : EnemyState
    {
        protected override void OnInit(IFsm<EnemyEntity> fsm)
        {
            base.OnInit(fsm);
            
            // attack lose target
            FsmTransition loseTransition = new FsmTransition(typeof(EnemyIdleState));
            loseTransition.AddCondition(new AttackLoseCondition(m_Enemy));
            AddTransition(loseTransition);
        }

        protected override void OnEnter(IFsm<EnemyEntity> fsm)
        {
            base.OnEnter(fsm);

            m_Enemy.agent.isStopped = true;
            m_Enemy.agent.velocity = Vector3.zero;
            
            m_Enemy.animator.SetBool("IsAttack", true);
            m_Enemy.animator.SetBool("IsDead", false);
            m_Enemy.animator.SetFloat("MoveSpeed", 0.0f);
        }

        protected override void OnUpdate(IFsm<EnemyEntity> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);

            if (m_Enemy.closestTarget)
            {
                m_Enemy.CachedTransform.rotation = Quaternion.LookRotation(
                    Vector3.ProjectOnPlane(m_Enemy.closestTarget.Position - m_Enemy.CachedTransform.position,
                        m_Enemy.CachedTransform.up), m_Enemy.CachedTransform.up);
            }
        }
    }
}