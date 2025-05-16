using GameFramework;
using GameFramework.ObjectPool;
using UnityEngine;

namespace ZHSM
{
    public class DamageTextObject : ObjectBase
    {
        public static DamageTextObject Create(object target)
        {
            DamageTextObject damageTextObject = ReferencePool.Acquire<DamageTextObject>();
            damageTextObject.Initialize(target);
            return damageTextObject;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            
            DamageText damageText = Target as DamageText;
            if (damageText) damageText.gameObject.SetActive(true);
        }

        protected override void OnUnspawn()
        {
            base.OnUnspawn();
            
            DamageText damageText = Target as DamageText;
            if (damageText) damageText.gameObject.SetActive(false);
        }

        protected override void Release(bool isShutdown)
        {
            DamageText damageText = (DamageText)Target;
            if (damageText == null) return;

            Object.Destroy(damageText.gameObject);
        }
    }
}