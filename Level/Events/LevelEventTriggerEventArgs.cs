using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    /// <summary>
    /// 关卡事件触发
    /// </summary>
    public class LevelEventTriggerEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(LevelEventTriggerEventArgs).GetHashCode();

        public LevelEventTriggerEventArgs()
        {
            UserData = null;
        }

        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public string EventName { get; private set; }

        public object UserData
        {
            get;
            private set;
        }

        public static LevelEventTriggerEventArgs Create(string eventName, object userData = null)
        {
            LevelEventTriggerEventArgs args = ReferencePool.Acquire<LevelEventTriggerEventArgs>();
            args.EventName = eventName;
            args.UserData = userData;
            
            return args;
        }

        public override void Clear()
        {
            EventName = null;
            UserData = null;
        }
    }
}