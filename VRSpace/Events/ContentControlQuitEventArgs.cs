using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    public class ContentControlQuitEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(ContentControlQuitEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static ContentControlQuitEventArgs Create()
        {
            return ReferencePool.Acquire<ContentControlQuitEventArgs>();
        }

        public override void Clear() { }
    }
}