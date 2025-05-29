using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    public class ContentControlPauseEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(ContentControlPauseEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static ContentControlPauseEventArgs Create()
        {
            return ReferencePool.Acquire<ContentControlPauseEventArgs>();
        }

        public override void Clear() { }
    }
}