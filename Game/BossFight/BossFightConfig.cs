using System.Collections.Generic;
using UnityEngine;

namespace ZHSM
{
    [System.Serializable]
    public class BossHealthStep
    {
        
    }
    
    [System.Serializable]
    public class BossFightConfig : ScriptableObject
    {
        public int bossId;
        public List<BossHealthStep> healthSteps;
    }
}