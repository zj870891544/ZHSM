using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.cfg;
using ZHSM.Enemy;

namespace ZHSM
{
    public class BeachDefenseGame : GameBase
    {
        public override GameMode GameMode => GameMode.BeachDefense;
        private BeachDefenseConfig m_DefenseConfig;
        private bool m_PrePatrolComplete = false;
        private Dictionary<int, WaveEnemyInfo> m_WaveEnemyDic = new Dictionary<int, WaveEnemyInfo>();
        private Dictionary<int, bool> m_TowerOccupyDictionary = new Dictionary<int, bool>();

        public override void Initialize(object gameData = null)
        {
            base.Initialize(gameData);
            
            GameEntry.Event.Subscribe(DefenseTowerOccupiedEventArgs.EventId, OnDefenseTowerOccupied);
            
            StopAllCoroutines();
        }

        public override void Shutdown()
        {
            base.Shutdown();
            
            GameEntry.Event.Unsubscribe(DefenseTowerOccupiedEventArgs.EventId, OnDefenseTowerOccupied);

            //清除Towers
            foreach (KeyValuePair<int,bool> kv in m_TowerOccupyDictionary)
            {
                if (GameEntry.Entity.HasEntity(kv.Key))
                {
                    GameEntry.Entity.HideEntity(kv.Key);
                }
            }

            //清除Enemy
            foreach (KeyValuePair<int,WaveEnemyInfo> kv in m_WaveEnemyDic)
            {
                if (GameEntry.Entity.HasEntity(kv.Key))
                {
                    GameEntry.Entity.HideEntity(kv.Key);
                }
            }
        }

        private void OnDefenseTowerOccupied(object sender, GameEventArgs e)
        {
            DefenseTowerOccupiedEventArgs ne = e as DefenseTowerOccupiedEventArgs;
            if (ne == null || GameOver) return;

            if (m_TowerOccupyDictionary.ContainsKey(ne.EntityId))
            {
                m_TowerOccupyDictionary[ne.EntityId] = true;

                int occupiedNum = 0;
                foreach (KeyValuePair<int,bool> kv in m_TowerOccupyDictionary) if (kv.Value) occupiedNum++;
                if (occupiedNum >= m_DefenseConfig.occupyCount) OnOccupyComplete();
            }
        }

        private void OnOccupyComplete()
        {
            RpcLevelComplete(waveIndex);
            ShowPortals();
        }

        protected override void OnInitializeSuccess(LevelConfig levelConfig)
        {
            base.OnInitializeSuccess(levelConfig);

            m_PrePatrolComplete = false;
            
            m_DefenseConfig = levelConfig as BeachDefenseConfig;
            if (m_DefenseConfig != null)
            {
                StartCoroutine(InitLevel());
            }
            else
            {
                Log.Error($"Load BeachDefenseConfig parse error: {m_DefenseConfig.name}");
            }
        }

        private IEnumerator InitLevel()
        {
            // 创建防御塔
            foreach (DefenseTowerConfig towerConfig in m_DefenseConfig.towers)
            {
                int entityId = GameEntry.Entity.GenerateSerialId();
                GameEntry.Entity.ShowDefenseTower(new DefenseTowerData(entityId,
                    towerConfig.entityId, CampType.PlayerDefenseTower, towerConfig.hp)
                {
                    Position = towerConfig.position,
                    Rotation = towerConfig.rotation,
                    Radius = towerConfig.radius,
                    OccupySpeed = towerConfig.occupySpeed
                });
                
                m_TowerOccupyDictionary.Add(entityId, false);

                yield return null;
            }
            
            //创建巡逻怪群
            m_WaveEnemyDic = new Dictionary<int, WaveEnemyInfo>();
            foreach (DefenseSpawnerPoint spawnerPoint in m_DefenseConfig.preSpawnerPoints)
            {
                EnemyCfg enemyCfg = tbEnemyCfg.GetOrDefault(spawnerPoint.enemyId);
                if (enemyCfg == null)
                {
                    Log.Error($"创建群怪点失败 EnemyId {spawnerPoint.enemyId} 未找到.");
                    continue;
                }

                WaveEnemyInfo waveEnemyInfo = null;
                if (!m_WaveEnemyDic.ContainsKey(spawnerPoint.enemyId))
                {
                    waveEnemyInfo = new WaveEnemyInfo
                    {
                        maxNum = spawnerPoint.count,
                        spawnedList = new List<int>(),
                        killedNum = 0,
                        probability = 1
                    };
                    m_WaveEnemyDic.Add(spawnerPoint.enemyId, waveEnemyInfo);
                }
                else
                {
                    waveEnemyInfo = m_WaveEnemyDic[spawnerPoint.enemyId];
                    waveEnemyInfo.maxNum += spawnerPoint.count;
                }

                for (int i = 0; i < spawnerPoint.count; i++)
                {
                    int entityId = GameEntry.Entity.GenerateSerialId();
                    GameEntry.Entity.ShowEnemy(new EnemyData(entityId, enemyCfg.EntityId,
                        CampType.Enemy, spawnerPoint.enemyId)
                    {
                        Position = AIUtility.GetInsidePosition(spawnerPoint.position, spawnerPoint.radius),
                        Rotation = Quaternion.identity,
                        PatrolEnable = true,
                        PatrolCenter = spawnerPoint.position,
                        PatrolRadius = spawnerPoint.radius
                    });
                    waveEnemyInfo.spawnedList.Add(entityId);
                    
                    yield return null;
                }
            }
            
            RpcLevelStart(levelId);
        }

        private IEnumerator StartWave()
        {
            m_WaveEnemyDic = new Dictionary<int, WaveEnemyInfo>();

            Log.Info($"关卡{levelId}  第[{waveIndex}]波开始 >>>");
            LevelWaveConfig levelWave = m_DefenseConfig.waves[waveIndex];
            foreach (LevelWaveEnemyConfig waveEnemy in levelWave.enemyList)
            {
                int enemyId = waveEnemy.enemyId;
                if (m_WaveEnemyDic.ContainsKey(enemyId))
                {
                    Log.Error($"{levelId}-{waveIndex}  当前波怪 {enemyId} 重复，已忽略 >>>");
                    continue;
                }
                
                m_WaveEnemyDic.Add(enemyId, new WaveEnemyInfo
                {
                    maxNum = waveEnemy.count,
                    spawnedList = new List<int>(),
                    killedNum = 0,
                    probability = waveEnemy.probability
                });
            }
            
            ClearLevelEvents();
            if (!string.IsNullOrEmpty(levelWave.preEvent)) AddLevelEvent(levelWave.preEvent);
            if (!string.IsNullOrEmpty(levelWave.postEvent)) AddLevelEvent(levelWave.postEvent);
            
            // 前置事件
            if (!string.IsNullOrEmpty(levelWave.preEvent))
            {
                Log.Info($"{levelId}-{waveIndex}  激活前置事件 >>>");
                RpcLevelEventActivate(levelWave.preEvent);
                
                Log.Info($"{levelId}-{waveIndex}  等待前置事件 >>>");
                yield return new WaitUntil(() => IsEventTriggered(levelWave.preEvent));
            }
            
            Log.Info($"关卡{levelId}-{waveIndex}  开始刷怪 >>>");
            while (true)
            {
                if (!TryGetSpawnEnemyId(out int enemyId)) break;

                if (m_WaveEnemyDic.TryGetValue(enemyId, out WaveEnemyInfo waveInfo))
                {
                    EnemyCfg enemyCfg = tbEnemyCfg.GetOrDefault(enemyId);
                    if (enemyCfg == null)
                    {
                        Log.Error($"关卡{levelId} 第{waveIndex}波 未找到怪：{enemyId}，跳过.");
                        continue;
                    }
                    
                    int entityId = GameEntry.Entity.GenerateSerialId();
                    GameEntry.Entity.ShowEnemy(new EnemyData(entityId, enemyCfg.EntityId, CampType.Enemy, enemyId)
                    {
                        Position = levelWave.spawnPoints[Random.Range(0, levelWave.spawnPoints.Count)],
                        Rotation = Quaternion.identity,
                        PatrolEnable = false
                    });
                    
                    waveInfo.spawnedList.Add(entityId);
                    
                    // Log.Info($"刷怪 {enemyId} >>> {waveInfo.spawnedList.Count}/{waveInfo.maxNum}");
                }
                else
                {
                    Log.Error($"关卡{levelId} 第{waveIndex}波 刷怪数据错误  {enemyId}");
                }
                
                yield return new WaitForSeconds(levelsCfg.Rate);
            }
            
            Log.Info($"关卡{levelId}-{waveIndex} 刷怪结束 >>> ");
        }

        private IEnumerator OnWaveComplete()
        {
            if (!m_PrePatrolComplete)
            {
                m_PrePatrolComplete = true;

                yield return new WaitForSeconds(3.0f);

                waveIndex = 0;
                StartCoroutine(StartWave());
                yield break;
            }
            
            // 后置事件
            LevelWaveConfig waveConfig = m_DefenseConfig.waves[waveIndex];
            if (!string.IsNullOrEmpty(waveConfig.postEvent))
            {
                Log.Info($"{levelId}-{waveIndex}  激活后置事件 >>>");
                RpcLevelEventActivate(waveConfig.postEvent);
                
                Log.Info($"{levelId}-{waveIndex}  等待后置事件 >>>");
                yield return new WaitUntil(() => IsEventTriggered(waveConfig.postEvent));
            }
            
            Log.Info($"{levelId}-{waveIndex} 一波结束 >>> ");
            
            waveIndex++;
            if (waveIndex > m_DefenseConfig.waves.Count - 1)
            {
                Log.Info($"{levelId}-{waveIndex} 最后一波结束 >>> ");
            }
            else
            {
                yield return new WaitForSeconds(5);
                
                StartCoroutine(StartWave());
            }
        }
        
        protected override void OnEnemyDead(object sender, GameEventArgs e)
        {
            EnemyDeadEventArgs ne = e as EnemyDeadEventArgs;
            if (ne == null) return;

            if (m_WaveEnemyDic.TryGetValue(ne.EnemyId, out WaveEnemyInfo waveInfo))
            {
                if (waveInfo.spawnedList.Contains(ne.EntityId))
                {
                    waveInfo.killedNum++;
                }
                
                // Log.Info($"enemy dead  {waveInfo.killedNum}/{waveInfo.spawnedList.Count}/{waveInfo.maxNum}");
                
                // 检查击杀数量
                foreach (KeyValuePair<int,WaveEnemyInfo> kv in m_WaveEnemyDic)
                {
                    if (kv.Value.spawnedList.Count < kv.Value.maxNum) return;//检查刷新数量
                    if (kv.Value.killedNum < kv.Value.maxNum) return;//检查击杀数量
                }
                StartCoroutine(OnWaveComplete());
            }
        }
        
        private bool TryGetSpawnEnemyId(out int enemyId)
        {
            List<int> enemyList = new List<int>();
            List<float> probabilityList = new List<float>();
            
            float totalProbability = 0f;
            foreach (KeyValuePair<int, WaveEnemyInfo> kv in m_WaveEnemyDic)
            {
                // 忽略已完成的怪
                if(kv.Value.spawnedList.Count >= kv.Value.maxNum) continue;

                totalProbability += kv.Value.probability;
                
                enemyList.Add(kv.Key);
                probabilityList.Add(totalProbability);
            }

            float resultP = Random.Range(0.0f, totalProbability);
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (resultP < probabilityList[i])
                {
                    enemyId = enemyList[i];
                    return true;
                }
            }

            enemyId = 0;
            return false;
        }
    }
}