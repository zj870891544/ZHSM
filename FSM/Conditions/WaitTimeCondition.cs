namespace ZHSM
{
    public class WaitTimeCondition : FsmCondition
    {
        private float timer;
        private float duration;
        
        public WaitTimeCondition(object owner, float waitTime) : base(owner)
        {
            duration = waitTime;
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