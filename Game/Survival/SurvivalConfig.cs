using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ZHSM
{
    [CreateAssetMenu(menuName = "VRUtils/关卡/割草配置", fileName = "level_x")]
    public class SurvivalConfig : LevelConfig
    {
        [LabelText("刷怪波")]
        [ListDrawerSettings(ShowIndexLabels = true, ShowItemCount = true)]
        public List<LevelWaveConfig> waves;

#if UNITY_EDITOR
        public override void DrawGizmos()
        {
            base.DrawGizmos();
            
            if(waves == null) return;

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
#endif
    }
}