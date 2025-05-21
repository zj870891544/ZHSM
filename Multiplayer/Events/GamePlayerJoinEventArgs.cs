using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    public class GamePlayerJoinEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(GamePlayerJoinEventArgs).GetHashCode();

        public GamePlayerJoinEventArgs()
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

        public static GamePlayerJoinEventArgs Create(NetworkPlayer player, object userData = null)
        {
            GamePlayerJoinEventArgs args = ReferencePool.Acquire<GamePlayerJoinEventArgs>();
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