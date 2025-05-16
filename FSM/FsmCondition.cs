using GameFramework;

namespace ZHSM
{
    public abstract class FsmCondition
    {
        protected FsmCondition(object owner) { }
        public abstract bool Check(float elapsedTime);
        public abstract void Reset();
    }
}