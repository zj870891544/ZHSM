using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    /// <summary>
    /// 加载游戏场景事件
    /// 网络状态主动控制，用于通知流程跳转场景场景
    /// </summary>
    public class NetworkChangeSceneEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(NetworkChangeSceneEventArgs).GetHashCode();

        public NetworkChangeSceneEventArgs()
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
        
        public int SceneId { get; private set; }

        public object UserData
        {
            get;
            private set;
        }

        public static NetworkChangeSceneEventArgs Create(int sceneId, object userData = null)
        {
            NetworkChangeSceneEventArgs args = ReferencePool.Acquire<NetworkChangeSceneEventArgs>();
            args.SceneId = sceneId;
            args.UserData = userData;
            
            return args;
        }

        public override void Clear()
        {
            SceneId = 0;
            UserData = null;
        }
    }
}