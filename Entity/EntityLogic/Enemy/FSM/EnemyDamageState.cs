using GameFramework.Fsm;
using UnityEngine;

namespace ZHSM.Enemy
{
    public class EnemyDamageState : EnemyState
    {
        private FsmTransition waitTransition;
        private bool beKnockUp = false;
        private Vector3 knockUpDir;

        protected override void OnEnter(IFsm<EnemyEntity> fsm)
        {
            base.OnEnter(fsm);
            
            m_Enemy.agent.isStopped = true;
            m_Enemy.agent.velocity = Vector3.zero;
            
            m_Enemy.animator.SetBool("IsAttack", false);
            m_Enemy.animator.SetBool("IsDead", false);
            m_Enemy.animator.SetFloat("MoveSpeed", 0.0f);
            m_Enemy.animator.Play("Take Damage");

            float damageDuration = 1.0f;
            
            float knockUpOffset = m_Enemy.damageRequestData.knockUp - m_Enemy.mass;
            if (knockUpOffset > 0)
            {
                beKnockUp = true;
                knockUpDir = m_Enemy.damageRequestData.knockUpDirection.normalized;
                
                damageDuration = Mathf.Max(damageDuration, knockUpOffset / m_Enemy.KnockUpSpeed);
            }
            
            waitTransition = new FsmTransition(typeof(EnemyIdleState));
            waitTransition.AddCondition(new WaitTimeCondition(m_Enemy, damageDuration));
            AddTransition(waitTransition);
        }
        
        protected override void OnUpdate(IFsm<EnemyEntity> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);

            if (beKnockUp)
            {
                m_Enemy.agent.Move(knockUpDir * elapseSeconds * m_Enemy.KnockUpSpeed);
            }
        }

        protected override void OnLeave(IFsm<EnemyEntity> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            
            RemoveTransition(waitTransition);
        }
    }
}