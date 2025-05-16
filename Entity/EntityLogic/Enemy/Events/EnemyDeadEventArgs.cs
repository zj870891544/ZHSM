
using GameFramework;
using GameFramework.Event;

namespace ZHSM.Enemy
{
    public class EnemyDeadEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(EnemyDeadEventArgs).GetHashCode();

        public EnemyDeadEventArgs()
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
        
        public int EnemyId { get; private set; }
        public int EntityId { get; private set; }
        public object UserData { get; private set; }

        public static EnemyDeadEventArgs Create(int enemyId, int entityId, object userData = null)
        {
            EnemyDeadEventArgs args = ReferencePool.Acquire<EnemyDeadEventArgs>();
            args.EnemyId = enemyId;
            args.EntityId = entityId;
            args.UserData = userData;
            
            return args;
        }

        public override void Clear()
        {
            EnemyId = 0;
            EntityId = 0;
            UserData = null;
        }
    }
}