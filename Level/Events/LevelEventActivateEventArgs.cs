using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    /// <summary>
    /// 关卡事件激活
    /// </summary>
    public class LevelEventActivateEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(LevelEventActivateEventArgs).GetHashCode();

        public LevelEventActivateEventArgs()
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

        public static LevelEventActivateEventArgs Create(string eventName, object userData = null)
        {
            LevelEventActivateEventArgs args = ReferencePool.Acquire<LevelEventActivateEventArgs>();
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