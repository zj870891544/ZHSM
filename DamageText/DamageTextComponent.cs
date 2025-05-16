using GameFramework.ObjectPool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class DamageTextComponent : GameFrameworkComponent
    {
        [Title("DamageText")]
        [SerializeField] private DamageText m_DamageTextPrefab = null;
        [SerializeField] private Transform m_DamageTextInstanceRoot = null;
        [SerializeField] private int m_DamageTextPoolCapacity = 16;
        
        private IObjectPool<DamageTextObject> m_DamageTextObjectPool = null;

        private void Start()
        {
            if (m_DamageTextInstanceRoot == null)
            {
                Log.Error("You must set HP bar instance root first.");
                return;
            }

            m_DamageTextObjectPool = GameEntry.ObjectPool.CreateSingleSpawnObjectPool<DamageTextObject>("DamageText", m_DamageTextPoolCapacity);
        }
        
        public void ShowDamageText(TargetableObject targetable, int damage)
        {
            if (targetable == null)
            {
                Log.Warning("Show damageText failure: targetable is invalid.");
                return;
            }
            
            DamageText damageText = null;
            DamageTextObject damageTextObject = m_DamageTextObjectPool.Spawn();
            if (damageTextObject != null)
            {
                damageText = (DamageText)damageTextObject.Target;
            }
            else
            {
                damageText = Instantiate(m_DamageTextPrefab);
                
                Transform hpBarTrans = damageText.GetComponent<Transform>();
                hpBarTrans.SetParent(m_DamageTextInstanceRoot);
                hpBarTrans.localScale = Vector3.one;
                
                m_DamageTextObjectPool.Register(DamageTextObject.Create(damageText), true);
            }
            
            damageText.Init(targetable, damage);
        }

        public void HideDamageText(DamageText damageText)
        {
            m_DamageTextObjectPool.Unspawn(damageText);
        }
    }
}