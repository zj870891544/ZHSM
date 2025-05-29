using GameFramework;
using GameFramework.Event;
using ZHSM.LogicServer;

namespace ZHSM
{
    public class B2LPlayersUpdateEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(B2LPlayersUpdateEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public SCB2LPlayerData PlayerData { get; private set; }
        public object UserData { get; private set; }

        public static B2LPlayersUpdateEventArgs Create(SCB2LPlayerData b2lPlayerData, object userData = null)
        {
            B2LPlayersUpdateEventArgs args = ReferencePool.Acquire<B2LPlayersUpdateEventArgs>();
            args.PlayerData = b2lPlayerData;
            args.UserData = userData;

            return args;
        }

        public override void Clear()
        {
            UserData = null;
        }
    }
}