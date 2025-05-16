using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.cfg;
using ZHSM.Enemy;

namespace ZHSM
{
    public class SurvivalGame : GameBase
    {
        public override GameMode GameMode => GameMode.Survival;

        private SurvivalConfig m_SurvivalConfig;
        private Dictionary<int, WaveEnemyInfo> m_WaveEnemyDic = new Dictionary<int, WaveEnemyInfo>();

        public override void Initialize(object gameData = null)
        {
            base.Initialize(gameData);
            
            StopAllCoroutines();
        }

        public override void Shutdown()
        {
            base.Shutdown();
            
            //清除Enemy
            foreach (KeyValuePair<int,WaveEnemyInfo> kv in m_WaveEnemyDic)
            {
                if (GameEntry.Entity.HasEntity(kv.Key))
                {
                    GameEntry.Entity.HideEntity(kv.Key);
                }
            }
        }

        protected override void OnInitializeSuccess(LevelConfig levelConfig)
        {
            base.OnInitializeSuccess(levelConfig);
            
            m_SurvivalConfig = levelConfig as SurvivalConfig;
            if (m_SurvivalConfig != null)
            {
                RpcLevelStart(levelId);
                StartCoroutine(StartWave());
            }
            else
            {
                Log.Error($"Load BeachDefenseConfig parse error: {levelConfig.name}");
            }
        }

        private IEnumerator StartWave()
        {
            m_WaveEnemyDic = new Dictionary<int, WaveEnemyInfo>();
            
            Log.Info($"关卡{levelId}  第[{waveIndex}]波开始 >>>");
            LevelWaveConfig levelWave = m_SurvivalConfig.waves[waveIndex];
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

                if (m_WaveEnemyDic.TryGetValue(enemyId, out WaveEnemyInfo enemyInfo))
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
                    
                    enemyInfo.spawnedList.Add(entityId);
                }
                else
                {
                    Log.Error($"关卡{levelId} 第{waveIndex}波 刷怪数据错误  {enemyId}");
                }
                
                yield return new WaitForSeconds(levelsCfg.Rate);
            }
            
            Log.Info($"关卡{levelId}-{waveIndex}  刷怪结束 >>>");
        }
        
        private IEnumerator OnWaveComplete()
        {
            // 后置事件
            LevelWaveConfig levelWave = m_SurvivalConfig.waves[waveIndex];
            if (!string.IsNullOrEmpty(levelWave.postEvent))
            {
                Log.Info($"{levelId}-{waveIndex}  激活后置事件 >>>");
                RpcLevelEventActivate(levelWave.postEvent);
                
                Log.Info($"{levelId}-{waveIndex}  等待后置事件 >>>");
                yield return new WaitUntil(() => IsEventTriggered(levelWave.postEvent));
            }
            
            Log.Info($"{levelId}-{waveIndex} 一波结束 >>> ");
            
            
            waveIndex++;
            if (waveIndex > m_SurvivalConfig.waves.Count - 1)
            {
                Log.Info($"{levelId} 关卡结束 >>> ");
                RpcLevelComplete(levelId);
                
                ShowPortals();
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