using System.Collections.Generic;
using GameFramework.Event;
using GameFramework.Fsm;
using MEC;
using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.cfg;
using ZHSM.Enemy;

namespace ZHSM.FSM
{
    public class BossSummonState : BossState
    {
        private TbEnemyCfg m_TbEnemyCfg;
        private Dictionary<int, WaveEnemyInfo> m_WaveEnemyDic;
        private int m_WaveIndex;
        private LevelWaveConfig m_WaveConfig;

        protected override void OnInit(IFsm<BossEntity> fsm)
        {
            base.OnInit(fsm);

            m_TbEnemyCfg = GameEntry.LubanTable.GetTbEnemyCfg();
        }

        protected override void OnEnter(IFsm<BossEntity> fsm)
        {
            base.OnEnter(fsm);
            
            Log.Info("Boss召唤小兵 >>> ");
            
            GameEntry.Event.Subscribe(EnemyDeadEventArgs.EventId, OnEnemyDead);

            m_WaveIndex = 0;
            
            Timing.RunCoroutine(StartWave());
        }

        protected override void OnLeave(IFsm<BossEntity> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            
            GameEntry.Event.Unsubscribe(EnemyDeadEventArgs.EventId, OnEnemyDead);
        }

        private IEnumerator<float> StartWave()
        {
            m_WaveEnemyDic = new Dictionary<int, WaveEnemyInfo>();
            
            Log.Info($"Boss战 第[{m_WaveIndex}]波开始 >>>");
            if (m_Boss.WaveConfigs == null || m_Boss.WaveConfigs.Count <= 0)
            {
                Log.Error("刷怪波未配置");
                yield break;
            }
            
            m_WaveConfig = m_Boss.WaveConfigs[m_WaveIndex];
            foreach (LevelWaveEnemyConfig waveEnemy in m_WaveConfig.enemyList)
            {
                int enemyId = waveEnemy.enemyId;
                if (m_WaveEnemyDic.ContainsKey(enemyId))
                {
                    Log.Error($"{m_WaveIndex}  怪 {enemyId} 重复，已忽略 >>>");
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

            yield return Timing.WaitForSeconds(1.0f);
            
            m_Boss.CachedAnimator.SetTrigger("SummonTrigger");

            while (true)
            {
                if (!TryGetSpawnEnemyId(out int enemyId)) break;

                if (m_WaveEnemyDic.TryGetValue(enemyId, out WaveEnemyInfo enemyInfo))
                {
                    EnemyCfg enemyCfg = m_TbEnemyCfg.GetOrDefault(enemyId);
                    if (enemyCfg == null)
                    {
                        Log.Error($"第{m_WaveIndex}波 未找到怪：{enemyId}，忽略 >>> ");
                        continue;
                    }

                    int entityId = GameEntry.Entity.GenerateSerialId();
                    GameEntry.Entity.ShowEnemy(new EnemyData(entityId, enemyCfg.EntityId, CampType.Enemy, enemyId)
                    {
                        Position = m_WaveConfig.spawnPoints[Random.Range(0, m_WaveConfig.spawnPoints.Count)],
                        Rotation = Quaternion.identity,
                        PatrolEnable = false
                    });
                    
                    enemyInfo.spawnedList.Add(entityId);
                }
                else
                {
                    Log.Error($"第{m_WaveIndex}波 刷怪数据错误  {enemyId}");
                }

                yield return Timing.WaitForSeconds(1.0f);
            }
            
            Log.Info("召唤结束，等待小怪击杀完毕 >>> ");
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
        
        private void OnEnemyDead(object sender, GameEventArgs e)
        {
            EnemyDeadEventArgs ne = e as EnemyDeadEventArgs;
            if(ne == null) return;
            
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
                Timing.RunCoroutine(OnWaveComplete());
            }
        }
        
        
        private IEnumerator<float> OnWaveComplete()
        {
            Log.Info($"第{m_WaveIndex}波结束 >>> ");
            
            m_WaveIndex++;
            if (m_WaveIndex > m_Boss.WaveConfigs.Count - 1)
            {
                OnSkillComplete();
            }
            else
            {
                yield return Timing.WaitForSeconds(1.0f);
                Timing.RunCoroutine(StartWave());
            }
        }
    }
}