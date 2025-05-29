using Mirror;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class GrenadeEntity : Entity
    {
        private GrenadeData m_GrenadeData;
        private NetworkGrenade m_NetworkGrenade;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_GrenadeData = userData as GrenadeData;
            if (m_GrenadeData == null)
            {
                Log.Error("GrenadeData is invalid in GrenadeEntity's OnShow.");
                return;
            }

            // 如果是服务器则生成网络对象
            if (NetworkServer.active)
                NetworkServer.Spawn(gameObject);

            m_NetworkGrenade = GetComponent<NetworkGrenade>();

            // 设置手雷弹属性参数
            if (m_NetworkGrenade != null)
            {
                m_NetworkGrenade.RpcSetGrenadeProperties(
                    (int)m_GrenadeData.GrenadeType,
                    m_GrenadeData.Damage,
                    m_GrenadeData.ExplosionRadius,
                    m_GrenadeData.EffectDuration,
                    m_GrenadeData.ExplosionEffectId,
                    m_GrenadeData.ExplosionSoundId
                );

                // 设置手榴弹所有者
                if (m_GrenadeData.Owner != null)
                {
                    m_NetworkGrenade.Owner = m_GrenadeData.Owner;
                }
            }
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);

            if (NetworkServer.active && gameObject != null)
                NetworkServer.UnSpawn(gameObject);
        }
    }
}