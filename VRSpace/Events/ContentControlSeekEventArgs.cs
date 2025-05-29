using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    public class ContentControlSeekEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(ContentControlSeekEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public string StageName { get; private set; }
        public string AreaName { get; private set; }
        public object UserData { get; private set; }

        public static ContentControlSeekEventArgs Create(string stageName, string areaName, object userData = null)
        {
            ContentControlSeekEventArgs args = ReferencePool.Acquire<ContentControlSeekEventArgs>();
            args.StageName = stageName;
            args.AreaName = areaName;
            args.UserData = userData;

            return args;
        }

        public override void Clear()
        {
            StageName = null;
            AreaName = null;
            UserData = null;
        }
    }
}