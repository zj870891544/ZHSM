using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    public class ContentControlStartEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(ContentControlStartEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static ContentControlStartEventArgs Create()
        {
            return ReferencePool.Acquire<ContentControlStartEventArgs>();
        }

        public override void Clear() { }
    }
}