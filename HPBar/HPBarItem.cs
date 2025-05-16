using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class HPBarItem : MonoBehaviour
    {
        private const float AnimationSeconds = 0.3f;
        private const float KeepSeconds = 0.4f;
        private const float FadeOutSeconds = 0.3f;
        
        [SerializeField] private Slider hpBar;

        public TargetableObject Owner { get; private set; }

        private Transform m_CachedTransform;
        private CanvasGroup m_CachedCanvasGroup = null;
        
        public void Init(TargetableObject owner, float fromHpRatio, float toHPRatio)
        {
            if (owner == null)
            {
                Log.Error("Owner is invalid.");
                return;
            }
            
            gameObject.SetActive(true);
            StopAllCoroutines();

            m_CachedCanvasGroup.alpha = 1f;
            hpBar.value = fromHpRatio;
            Owner = owner;

            Refresh();

            StartCoroutine(HPBarCo(toHPRatio, AnimationSeconds, KeepSeconds, FadeOutSeconds));
        }

        public bool Refresh()
        {
            if (m_CachedCanvasGroup.alpha <= 0f) return false;
            
            if (Owner && !Owner.IsDead)
            {
                m_CachedTransform.position = Owner.HPBarPoint.position;
                m_CachedTransform.LookAt(Camera.main.transform.position);
            }

            return true;
        }
        
        public void Reset()
        {
            StopAllCoroutines();
            
            m_CachedCanvasGroup.alpha = 1f;
            hpBar.value = 1f;
            Owner = null;
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            m_CachedTransform = transform;
            m_CachedCanvasGroup = GetComponentInChildren<CanvasGroup>();
        }
        
        private IEnumerator HPBarCo(float value, float animationDuration, float keepDuration, float fadeOutDuration)
        {
            yield return hpBar.SmoothValue(value, animationDuration);
            yield return new WaitForSeconds(keepDuration);
            yield return m_CachedCanvasGroup.FadeToAlpha(0f, fadeOutDuration);
        }
    }
}