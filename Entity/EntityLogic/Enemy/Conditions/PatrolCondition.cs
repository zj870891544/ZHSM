using UnityEngine;

namespace ZHSM.Enemy.Conditions
{
    public class PatrolCondition : FsmCondition
    {
        private EnemyEntity enemy;
        private float patrolTimer;
        private float patrolDistance;
        
        public PatrolCondition(object owner, float patrolMinDistance) : base(owner)
        {
            enemy = owner as EnemyEntity;
            patrolDistance = patrolMinDistance;
            
            patrolTimer = 0.0f;
            enemy.ResetPatrolTarget();
        }

        public override bool Check(float elapsedTime)
        {
            // 防止卡怪
            patrolTimer += elapsedTime;
            if (patrolTimer >= 10.0f) return true;
            
            return Vector3.Distance(enemy.CachedTransform.position, enemy.PatrolPosition) <= patrolDistance;
        }

        public override void Reset()
        {
            patrolTimer = 0.0f;
            enemy.ResetPatrolTarget();
        }
    }
}