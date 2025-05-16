using Mirror;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class BossEntity : Entity
    {
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            Log.Info("[Entity] OnShow");
            
            NetworkServer.Spawn(gameObject);
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            
            Log.Info("[Entity] OnHide");
            NetworkServer.UnSpawn(gameObject);
        }
    }
}