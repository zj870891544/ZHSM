using GameFramework;
using GameFramework.Event;

namespace ZHSM
{
    /// <summary>
    /// 防御塔被占领事件
    /// </summary>
    public class DefenseTowerOccupiedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(DefenseTowerOccupiedEventArgs).GetHashCode();

        public DefenseTowerOccupiedEventArgs()
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
        
        public int EntityId { get; private set; }
        public object UserData { get; private set; }

        public static DefenseTowerOccupiedEventArgs Create(int enemyId, int entityId, object userData = null)
        {
            DefenseTowerOccupiedEventArgs args = ReferencePool.Acquire<DefenseTowerOccupiedEventArgs>();
            args.EntityId = entityId;
            args.UserData = userData;
            
            return args;
        }

        public override void Clear()
        {
            EntityId = 0;
            UserData = null;
        }
    }
}