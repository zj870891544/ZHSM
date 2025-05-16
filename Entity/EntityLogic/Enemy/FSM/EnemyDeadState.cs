using UnityEngine;
using GameFramework.Fsm;

namespace ZHSM.Enemy
{
    public class EnemyDeadState : FsmState<EnemyEntity>
    {
        private const float HIDE_DELAY = 3.0f;

        private EnemyEntity m_Enemy;
        private bool isHide = false;
        private float delayHideTimer = 0.0f;
        
        private bool beKnockUp = false;
        private Vector3 knockUpDir;
        private float knockDuration;
        private float knockTimer;

        protected override void OnInit(IFsm<EnemyEntity> fsm)
        {
            base.OnInit(fsm);
            
            m_Enemy = fsm.Owner;
        }

        protected override void OnEnter(IFsm<EnemyEntity> fsm)
        {
            base.OnEnter(fsm);
            
            fsm.Owner.agent.isStopped = true;
            fsm.Owner.agent.velocity = Vector3.zero;
            fsm.Owner.agent.stoppingDistance = 0.0f;
            
            fsm.Owner.animator.SetBool("IsDead", true);
            
            isHide = false;
            delayHideTimer = 0.0f;
            
            float knockUpOffset = m_Enemy.damageRequestData.knockUp - m_Enemy.mass;
            if (knockUpOffset > 0)
            {
                knockUpDir = m_Enemy.damageRequestData.knockUpDirection.normalized;
                knockDuration = knockUpOffset / m_Enemy.KnockUpSpeed;
                knockTimer = 0.0f;
                beKnockUp = true;
            }
        }

        protected override void OnUpdate(IFsm<EnemyEntity> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);

            if (isHide) return;
            
            delayHideTimer += elapseSeconds;
            if (delayHideTimer >= HIDE_DELAY)
            {
                isHide = true;

                GameEntry.Entity.HideEntity(fsm.Owner);
                return;
            }

            if (beKnockUp)
            {
                m_Enemy.agent.Move(knockUpDir * elapseSeconds * m_Enemy.KnockUpSpeed);
                
                knockTimer += elapseSeconds;
                if (knockTimer >= knockDuration) beKnockUp = false;
            }
        }
    }
}