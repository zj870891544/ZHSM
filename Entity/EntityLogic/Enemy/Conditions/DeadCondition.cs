using UnityEngine;

namespace ZHSM.Enemy.Conditions
{
    public class DeadCondition : FsmCondition
    {
        private EnemyEntity enemy;
        
        public DeadCondition(object owner) : base(owner)
        {
            enemy = owner as EnemyEntity;
        }

        public override bool Check(float elapsedTime)
        {
            return enemy.IsDead;
        }

        public override void Reset()
        {
            
        }
    }
}