using GameFramework;
using GameFramework.Event;
using Mirror;

namespace ZHSM
{
    /// <summary>
    /// 服务端创建玩家
    /// </summary>
    public class ServerAddPlayerEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(ServerAddPlayerEventArgs).GetHashCode();

        public ServerAddPlayerEventArgs()
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
        
        public NetworkConnectionToClient Connection { get; private set; }

        public object UserData
        {
            get;
            private set;
        }

        public static ServerAddPlayerEventArgs Create(NetworkConnectionToClient conn, object userData = null)
        {
            ServerAddPlayerEventArgs args = ReferencePool.Acquire<ServerAddPlayerEventArgs>();
            args.Connection = conn;
            args.UserData = userData;
            
            return args;
        }

        public override void Clear()
        {
            UserData = null;
        }
    }
}