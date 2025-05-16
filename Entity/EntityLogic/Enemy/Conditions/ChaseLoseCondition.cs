using UnityEngine;

namespace ZHSM.Enemy.Conditions
{
    public class ChaseLoseCondition : FsmCondition
    {
        private EnemyEntity enemy;
        
        public ChaseLoseCondition(object owner) : base(owner)
        {
            enemy = owner as EnemyEntity;
        }

        public override bool Check(float elapsedTime)
        {
            if (!enemy.closestTarget || enemy.closestTarget.IsDead) return true;

            if (enemy.PatrolEnable)
            {
                return Vector3.Distance(enemy.CachedTransform.position, enemy.closestTarget.Position) >
                       enemy.followDistance;
            }

            return false;
        }

        public override void Reset()
        {
            
        }
    }
}