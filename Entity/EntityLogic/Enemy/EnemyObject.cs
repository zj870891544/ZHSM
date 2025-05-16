using GameFramework;
using GameFramework.ObjectPool;
using UnityEngine;

namespace ZHSM.Enemy
{
    public class EnemyObject : ObjectBase
    {
        public static EnemyObject Create(object target)
        {
            EnemyObject enemyObject = ReferencePool.Acquire<EnemyObject>();
            enemyObject.Initialize(target);
            return enemyObject;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();

            Debug.Log("enemy OnSpawn >>> ");
        }

        protected override void OnUnspawn()
        {
            base.OnUnspawn();
            
            Debug.Log("enemy OnUnspawn >>> ");
        }

        protected override void Release(bool isShutdown)
        {
            EnemyEntity enemy = (EnemyEntity)Target;
            if (enemy == null) return;

            Object.Destroy(enemy.gameObject);
        }
    }
}