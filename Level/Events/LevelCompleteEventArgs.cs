using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    public class LevelCompleteEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(LevelCompleteEventArgs).GetHashCode();

        public LevelCompleteEventArgs()
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
        
        public int LevelIndex { get; private set; }

        public object UserData
        {
            get;
            private set;
        }

        public static LevelCompleteEventArgs Create(int levelIndex, object userData = null)
        {
            LevelCompleteEventArgs args = ReferencePool.Acquire<LevelCompleteEventArgs>();
            args.LevelIndex = levelIndex;
            args.UserData = userData;
            
            return args;
        }

        public override void Clear()
        {
            LevelIndex = 0;
            UserData = null;
        }
    }
}