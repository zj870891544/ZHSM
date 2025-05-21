using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    /// <summary>
    /// 加载游戏场景事件
    /// 网络状态主动控制，用于通知流程跳转场景场景
    /// </summary>
    public class MultiPlayerChangeSceneEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(MultiPlayerChangeSceneEventArgs).GetHashCode();

        public MultiPlayerChangeSceneEventArgs()
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

        public static MultiPlayerChangeSceneEventArgs Create(int sceneId, object userData = null)
        {
            MultiPlayerChangeSceneEventArgs args = ReferencePool.Acquire<MultiPlayerChangeSceneEventArgs>();
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