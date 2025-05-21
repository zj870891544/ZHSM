using UnityEngine;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class EffectEntity : Entity
    {
        [SerializeField] private EffectData m_EffectData;

        private float m_ElapseSeconds = 0f;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_EffectData = userData as EffectData;
            if (m_EffectData == null)
            {
                Log.Error("Effect data is invalid.");
                return;
            }

            m_ElapseSeconds = 0f;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (m_EffectData.AttachPoint)
            {
                CachedTransform.SetPositionAndRotation(m_EffectData.AttachPoint.position, m_EffectData.AttachPoint.rotation);
            }

            m_ElapseSeconds += elapseSeconds;
            if (m_ElapseSeconds >= m_EffectData.KeepTime)
            {
                GameEntry.Entity.HideEntity(this);
            }
        }
    }
}