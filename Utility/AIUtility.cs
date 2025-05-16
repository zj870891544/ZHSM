using UnityEngine;

namespace ZHSM
{
    public static class AIUtility
    {
        public static Vector3 GetInsidePosition(Vector3 center, float radius)
        {
            Vector2 unitPos = Random.insideUnitCircle * radius;
            return center + new Vector3(unitPos.x, 0, unitPos.y);
        }
    }
}