using Unity.VisualScripting;
using ZHSM.cfg;

namespace ZHSM.Enemy.Conditions
{
    public class DamageCondition : FsmCondition
    {
        private EnemyEntity enemy;
        
        public DamageCondition(object owner) : base(owner)
        {
            enemy = owner as EnemyEntity;
        }

        public override bool Check(float elapsedTime)
        {
            if (!enemy.DamageRequested) return false;

            return true;
        }

        public override void Reset()
        {
            
        }
    }
}