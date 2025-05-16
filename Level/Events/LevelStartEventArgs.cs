using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    public class LevelStartEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(LevelStartEventArgs).GetHashCode();

        public LevelStartEventArgs()
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

        public static LevelStartEventArgs Create(int levelIndex, object userData = null)
        {
            LevelStartEventArgs args = ReferencePool.Acquire<LevelStartEventArgs>();
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