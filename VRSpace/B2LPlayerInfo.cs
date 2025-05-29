using GameFramework;

namespace ZHSM
{
    public class B2LPlayerInfo : IReference
    {
        public string PlayerId;
        public int EntityId;
        public B2LPlayerEntity Entity;
        
        public void Clear()
        {
            PlayerId = string.Empty;
            
            GameEntry.Entity.HideEntity(EntityId);
            EntityId = 0;
        }
    }
}