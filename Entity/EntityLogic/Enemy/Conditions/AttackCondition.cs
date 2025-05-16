namespace ZHSM.Enemy.Conditions
{
    public class AttackCondition : FsmCondition
    {
        private EnemyEntity enemy;
        
        public AttackCondition(object owner) : base(owner)
        {
            enemy = owner as EnemyEntity;
        }

        public override bool Check(float elapsedTime)
        {
            return enemy.CheckDistance(enemy.attackDistance);
        }

        public override void Reset()
        {
            
        }
    }
}