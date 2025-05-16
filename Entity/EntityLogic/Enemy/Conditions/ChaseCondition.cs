using UnityEngine;

namespace ZHSM.Enemy.Conditions
{
    public class ChaseCondition : FsmCondition
    {
        private EnemyEntity enemy;
        
        public ChaseCondition(object owner) : base(owner)
        {
            enemy = owner as EnemyEntity;
        }

        public override bool Check(float elapsedTime)
        {
            if (enemy.PatrolEnable)
            {
                return enemy.CheckDistance(enemy.followDistance);
            }
            
            return enemy.closestTarget != null;
        }

        public override void Reset()
        {
            
        }
    }
}