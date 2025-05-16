using UnityEngine;

namespace ZHSM.Enemy.Conditions
{
    public class AttackLoseCondition : FsmCondition
    {
        private EnemyEntity enemy;
        
        public AttackLoseCondition(object owner) : base(owner)
        {
            enemy = owner as EnemyEntity;
        }

        public override bool Check(float elapsedTime)
        {
            if(!enemy.closestTarget || enemy.closestTarget.IsDead) return true;

            return Vector3.Distance(enemy.CachedTransform.position, enemy.closestTarget.Position) > enemy.attackDistance;
        }

        public override void Reset()
        {
            
        }
    }
}