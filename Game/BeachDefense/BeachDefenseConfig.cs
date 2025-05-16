using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ZHSM
{
    [Serializable]
    public class DefenseTowerConfig
    {
        public int entityId;
        [LabelText("坐标")]
        public Vector3 position = Vector3.zero;
        [LabelText("旋转朝向")]
        public Quaternion rotation = Quaternion.identity;
        [LabelText("初始血量")]
        public int hp;
        [LabelText("交互半径")]
        public float radius;
        [LabelText("占领速度")]
        public float occupySpeed = 1.0f;
    }
    
    [Serializable]
    public class DefenseSpawnerPoint
    {
        [LabelText("坐标")]
        public Vector3 position;
        [LabelText("刷怪ID")]
        public int enemyId;
        [LabelText("刷怪数量")]
        public int count;
        [LabelText("刷怪半径")]
        public float radius;
    }
    
    [CreateAssetMenu(menuName = "VRUtils/关卡/抢滩登陆配置", fileName = "level_x")]
    public class BeachDefenseConfig : LevelConfig
    {
        [LabelText("占领防御塔数量")]
        [MinValue(1)]
        public int occupyCount;
        
        [LabelText("防御塔")]
        public List<DefenseTowerConfig> towers;
        
        [LabelText("群怪刷新点")]
        public List<DefenseSpawnerPoint> preSpawnerPoints;
        
        [ListDrawerSettings(ShowIndexLabels = true)]
        public List<LevelWaveConfig> waves;
        
#if UNITY_EDITOR
        public override void DrawGizmos()
        {
            base.DrawGizmos();

            if (towers != null)
            {
                Handles.color = Color.yellow;
                for (int i = 0; i < towers.Count; i++)
                {
                    DefenseTowerConfig towerConfig = towers[i];
                    towerConfig.position = Handles.FreeMoveHandle(towerConfig.position,
                        towerConfig.radius, Vector3.zero, Handles.CircleHandleCap);
                    Handles.ArrowHandleCap(i, towerConfig.position, towerConfig.rotation, towerConfig.radius,
                        EventType.Repaint);
                    
                    Handles.Label(towerConfig.position, $"tower-{i}");
                }
            }

            if (preSpawnerPoints != null)
            {
                Handles.color = new Color(1.0f, 0, 0, 0.2f);
                for (int i = 0; i < preSpawnerPoints.Count; i++)
                {
                    DefenseSpawnerPoint spawnerPoint = preSpawnerPoints[i];
                    
                    spawnerPoint.position = Handles.FreeMoveHandle(spawnerPoint.position,
                        spawnerPoint.radius, Vector3.zero, Handles.SphereHandleCap);
                }
            }
            
            if (waves != null)
            {
                Handles.color = Color.red;
                for (int i = 0; i < waves.Count; i++)
                {
                    LevelWaveConfig waveConfig = waves[i];
                    if(waveConfig.spawnPoints == null) continue;
                    
                    for (int j = 0; j < waveConfig.spawnPoints.Count; j++)
                    {
                        float size = HandleUtility.GetHandleSize(waveConfig.spawnPoints[j]) * 0.2f;
                        waveConfig.spawnPoints[j] = Handles.FreeMoveHandle(waveConfig.spawnPoints[j],
                            size, Vector3.zero, Handles.CircleHandleCap);
                    
                        Handles.Label(waveConfig.spawnPoints[j], $"{i}-{j}");
                    }
                }
            }
        }
#endif
    }
}