using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    public class ContentControlResumeEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(ContentControlResumeEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static ContentControlResumeEventArgs Create()
        {
            return ReferencePool.Acquire<ContentControlResumeEventArgs>();
        }

        public override void Clear() { }
    }
}