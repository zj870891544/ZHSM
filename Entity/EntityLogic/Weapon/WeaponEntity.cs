using Mirror;
using Mirror.SimpleWeb;

namespace ZHSM
{
    public class WeaponEntity : Entity
    {
        private WeaponData m_WeaponData;
        private NetworkWeapon m_NetworkWeapon;
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_WeaponData = userData as WeaponData;
            if (m_WeaponData == null)
            {
                Log.Error("WeaponData is invalid in WeaponEntity's OnShow.");
                return;
            }
            
            if (NetworkServer.active)
                NetworkServer.Spawn(gameObject, m_WeaponData.Connection);
            
            m_NetworkWeapon = GetComponent<NetworkWeapon>();
            m_NetworkWeapon.RpcSetWeaponId(m_NetworkWeapon.connectionToClient, m_WeaponData.WeaponId);
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            
            if (NetworkServer.active)
                NetworkServer.UnSpawn(gameObject);
        }
    }
}