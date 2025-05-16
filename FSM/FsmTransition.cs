using System;
using System.Collections.Generic;

namespace ZHSM
{
    public class FsmTransition
    {
        private List<FsmCondition> m_Conditions = new List<FsmCondition>();

        public FsmTransition(Type stateType)
        {
            StateType = stateType;
        }

        public Type StateType { get; private set; }
        
        public void AddCondition(FsmCondition condition) { m_Conditions.Add(condition); }

        public bool CheckConditions(float elapsedTime)
        {
            foreach (var condition in m_Conditions)
            {
                if (!condition.Check(elapsedTime)) return false;
            }
            
            return m_Conditions.Count > 0;
        }

        public void Reset()
        {
            foreach (var condition in m_Conditions)
            {
                condition.Reset();
            }
        }
    }
}