using System.Collections.Generic;
using GameFramework.ObjectPool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class HPBarComponent : GameFrameworkComponent
    {
        [Title("HPBar")]
        [SerializeField] private HPBarItem m_HPBarItemPrefab = null;
        [SerializeField] private Transform m_HPBarInstanceRoot = null;
        [SerializeField] private int m_InstancePoolCapacity = 16;
        
        private IObjectPool<HPBarItemObject> m_HPBarItemObjectPool = null;
        private List<HPBarItem> m_ActiveHPBarItems = null;
        
        private void Start()
        {
            if (m_HPBarInstanceRoot == null)
            {
                Log.Error("You must set HP bar instance root first.");
                return;
            }

            m_HPBarItemObjectPool = GameEntry.ObjectPool.CreateSingleSpawnObjectPool<HPBarItemObject>("HPBarItem", m_InstancePoolCapacity);
            m_ActiveHPBarItems = new List<HPBarItem>();
        }
        
        private void Update()
        {
            for (int i = m_ActiveHPBarItems.Count - 1; i >= 0; i--)
            {
                HPBarItem hpBarItem = m_ActiveHPBarItems[i];
                if (hpBarItem.Refresh()) continue;

                HideHPBar(hpBarItem);
            }
        }
        
        public void ShowHPBar(TargetableObject targetable, float fromHPRatio, float toHPRatio)
        {
            if (targetable == null)
            {
                Log.Warning("Targetable is invalid.");
                return;
            }

            HPBarItem hpBarItem = GetActiveHPBarItem(targetable);
            if (hpBarItem == null)
            {
                hpBarItem = CreateHPBarItem(targetable);
                m_ActiveHPBarItems.Add(hpBarItem);
            }

            hpBarItem.Init(targetable, fromHPRatio, toHPRatio);
        }
        
        private void HideHPBar(HPBarItem hpBarItem)
        {
            hpBarItem.Reset();
            m_ActiveHPBarItems.Remove(hpBarItem);
            m_HPBarItemObjectPool.Unspawn(hpBarItem);
        }
        
        private HPBarItem GetActiveHPBarItem(TargetableObject targetableObject)
        {
            if (targetableObject == null)
            {
                return null;
            }

            for (int i = 0; i < m_ActiveHPBarItems.Count; i++)
            {
                if (m_ActiveHPBarItems[i].Owner == targetableObject)
                {
                    return m_ActiveHPBarItems[i];
                }
            }

            return null;
        }
        
        private HPBarItem CreateHPBarItem(TargetableObject targetableObject)
        {
            HPBarItem hpBarItem = null;
            HPBarItemObject hpBarItemObject = m_HPBarItemObjectPool.Spawn();
            if (hpBarItemObject != null)
            {
                hpBarItem = (HPBarItem)hpBarItemObject.Target;
            }
            else
            {
                hpBarItem = Instantiate(m_HPBarItemPrefab);
                
                Transform hpBarTrans = hpBarItem.GetComponent<Transform>();
                hpBarTrans.SetParent(m_HPBarInstanceRoot);
                hpBarTrans.localScale = Vector3.one;
                
                m_HPBarItemObjectPool.Register(HPBarItemObject.Create(hpBarItem), true);
            }

            return hpBarItem;
        }
    }
}