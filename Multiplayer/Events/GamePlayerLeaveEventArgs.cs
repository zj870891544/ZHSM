using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    public class GamePlayerLeaveEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(GamePlayerLeaveEventArgs).GetHashCode();

        public GamePlayerLeaveEventArgs()
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
        
        public NetworkPlayer Player { get; private set; }

        public object UserData
        {
            get;
            private set;
        }

        public static GamePlayerLeaveEventArgs Create(NetworkPlayer player, object userData = null)
        {
            GamePlayerLeaveEventArgs args = ReferencePool.Acquire<GamePlayerLeaveEventArgs>();
            args.Player = player;
            args.UserData = userData;
            
            return args;
        }

        public override void Clear()
        {
            UserData = null;
        }
    }
}