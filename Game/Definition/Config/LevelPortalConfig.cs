using UnityEngine;

namespace ZHSM
{
    [System.Serializable]
    public class LevelPortalConfig
    {
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        public int nextLevel;
    }
}