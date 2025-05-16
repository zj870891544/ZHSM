using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ZHSM
{
    [System.Serializable]
    public class LevelWaveEnemyConfig
    {
        public int enemyId;
        public int count;
        public float probability;
    }
    
    [System.Serializable]
    public class LevelWaveConfig
    {
        [PropertySpace(SpaceBefore = 10, SpaceAfter = 0)]
        [ListDrawerSettings(ShowIndexLabels = true)]
        public List<Vector3> spawnPoints;
        [ListDrawerSettings(ShowIndexLabels = true)]
        public List<LevelWaveEnemyConfig> enemyList;
        public string preEvent;
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 10)]
        public string postEvent;
    }
}