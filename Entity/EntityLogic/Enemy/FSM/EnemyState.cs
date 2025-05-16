using System.Collections.Generic;
using GameFramework.Fsm;
using UnityEngine;
using ZHSM.Enemy.Conditions;

namespace ZHSM.Enemy
{
    public class EnemyState : FsmState<EnemyEntity>
    {
        protected EnemyEntity m_Enemy;
        
        private List<FsmTransition> m_Transitions = new List<FsmTransition>();

        protected void AddTransition(FsmTransition transition)
        {
            m_Transitions.Add(transition);
        }

        protected void RemoveTransition(FsmTransition transition)
        {
            m_Transitions.Remove(transition);
        }
        
        protected override void OnInit(IFsm<EnemyEntity> fsm)
        {
            base.OnInit(fsm);
            
            m_Enemy = fsm.Owner;
            
            // take damage
            FsmTransition damageTransition = new FsmTransition(typeof(EnemyDamageState));
            damageTransition.AddCondition(new DamageCondition(m_Enemy));
            AddTransition(damageTransition);

            // dead
            FsmTransition deadTransition = new FsmTransition(typeof(EnemyDeadState));
            deadTransition.AddCondition(new DeadCondition(m_Enemy));
            AddTransition(deadTransition);
        }

        protected override void OnUpdate(IFsm<EnemyEntity> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);

            foreach (FsmTransition transition in m_Transitions)
            {
                if (transition.CheckConditions(elapseSeconds))
                {
                    ChangeState(fsm, transition.StateType);
                    return;
                }
            }
        }

        protected override void OnLeave(IFsm<EnemyEntity> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            
            foreach (FsmTransition transition in m_Transitions)
            {
                transition.Reset();
            }
        }
    }
}