using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    public class StartMultiPlayerEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(StartMultiPlayerEventArgs).GetHashCode();

        public StartMultiPlayerEventArgs()
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

        public bool IsHost { get; private set; }
        public string ServerIP { get; private set; }
        public int ServerPort { get; private set; }
        public object UserData { get; private set; }

        public static StartMultiPlayerEventArgs Create(bool isHost, string serverIp, int serverPort, object userData = null)
        {
            StartMultiPlayerEventArgs args = ReferencePool.Acquire<StartMultiPlayerEventArgs>();
            args.IsHost = isHost;
            args.ServerIP = serverIp;
            args.ServerPort = serverPort;
            args.UserData = userData;
            
            return args;
        }

        public override void Clear()
        {
            IsHost = false;
            ServerIP = null;
            ServerPort = 0;
            UserData = null;
        }
    }
}