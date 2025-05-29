using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    public class ContentControlBodyHeightEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(ContentControlBodyHeightEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static ContentControlBodyHeightEventArgs Create()
        {
            return ReferencePool.Acquire<ContentControlBodyHeightEventArgs>();
        }

        public override void Clear() { }
    }
}