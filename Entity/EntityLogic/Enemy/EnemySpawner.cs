using System.Collections.Generic;
using GameFramework.Resource;
using Mirror;
using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.cfg;
using Random = UnityEngine.Random;

namespace ZHSM.Enemy
{
    public class EnemySpawner : NetworkBehaviour
    {
        public static int SERIAL_ID = 0;
        
        [SerializeField] private int enemyId;
        [SerializeField] private int maxSpawnNum = 3;
        [SerializeField] private float spawnRate = 1.0f;
        [SerializeField] private float spawnRadius = 0.5f;
        [SerializeField] private float patrolRadius = 1.0f;

        private EnemyEntity enemyPrefab;
        private List<EnemyEntity> enemyList = new List<EnemyEntity>();
        
        public void OnEnemyDestroy(EnemyEntity enemyObj)
        {
            if (enemyList.Contains(enemyObj)) enemyList.Remove(enemyObj);
        }
        
        public Vector3 GetPatrolPosition()
        {
            return GetInsidePosition(patrolRadius);
        }

        private Vector3 GetInsidePosition(float radius)
        {
            Vector2 unitPos = Random.insideUnitCircle * radius;
            return transform.position + new Vector3(unitPos.x, 0, unitPos.y);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, patrolRadius);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            
            TbEnemyCfg tbEnemyCfg = GameEntry.LubanTable.GetTbEnemyCfg();
            EnemyCfg enemyCfg = tbEnemyCfg.GetOrDefault(enemyId);
            
            string enemyAssetPath = AssetUtility.GetEnemyAsset(enemyCfg.Id);
            GameEntry.Resource.LoadAsset(enemyAssetPath, new LoadAssetCallbacks(
                (assetName, asset, duration, data) =>
                {
                    Debug.Log("load enemy asset success...");
                    
                    GameObject enemyObj = asset as GameObject;
                    if (enemyObj) enemyPrefab = enemyObj.GetComponent<EnemyEntity>();
                }, (assetName, status, message, data) =>
                {
                    Log.Error($"load enemy asset {assetName} error: {message}");
                }));
            
            InvokeRepeating(nameof(SpawnEnemyLoop), spawnRate, spawnRate);
        }

        private void SpawnEnemyLoop()
        {
            if (!NetworkServer.active) return;
            
            if (enemyList.Count < maxSpawnNum)
            {
                // GetInsidePosition(spawnRadius)
                EnemyEntity enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
                // enemy.Initialize(CampType.Enemy, enemyId);
                
                NetworkServer.Spawn(enemy.gameObject);
                
                enemyList.Add(enemy);
            }
        }
    }
}