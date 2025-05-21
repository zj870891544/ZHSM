using DG.Tweening;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class CircleIndicator : Entity
    {
        private CircleIndicatorData m_IndicatorData;
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            m_IndicatorData = userData as CircleIndicatorData;
            if (m_IndicatorData == null)
            {
                Log.Error("CircleIndicatorData is invalid.");
                return;
            }
            
            CachedTransform.localScale = Vector3.zero;
            CachedTransform.DOScale(m_IndicatorData.Radius, 0.5f).SetEase(Ease.OutBack);
        }
    }
}