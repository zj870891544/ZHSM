using Mirror;
using Mirror.SimpleWeb;
using UnityEngine;

namespace ZHSM
{
    public class ShieldEntity : WeaponEntity
    {
        private ShieldData m_ShieldData;
        private NetworkShield m_NetworkShield;
        
        //盾牌模型组件
        private GameObject m_shieldModel;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            //获取盾牌模型
            m_shieldModel = transform.Find("Model").gameObject;
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_ShieldData = userData as ShieldData;
            if (m_ShieldData == null)
            {
                Log.Error("ShieldData is null in ShieldEntity's Show.");
                return;
            }

            if (NetworkServer.active)
                NetworkServer.Spawn(gameObject,m_ShieldData.Connection);

            // 获取网络盾牌组件
            m_NetworkShield = GetComponent<NetworkShield>();
            if (m_NetworkShield == null)
            {
                Log.Error("NetworkShield component not found on ShieldEntity.");
                return;
            }
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);

            if (NetworkServer.active)
                NetworkServer.UnSpawn(gameObject);

        }

    }
}



