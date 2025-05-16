using UnityEngine;

namespace ZHSM
{
    public class WaitRandomTimeCondition : FsmCondition
    {
        private float timer;
        private float duration;
        
        public WaitRandomTimeCondition(object owner, float min, float max) : base(owner)
        {
            duration = Random.Range(min, max);
            timer = 0.0f;
        }

        public override bool Check(float elapsedTime)
        {
            timer += elapsedTime;
            return timer >= duration;
        }

        public override void Reset()
        {
            timer = 0.0f;
        }
    }
}