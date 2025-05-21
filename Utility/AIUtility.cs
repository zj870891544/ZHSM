using System.Collections.Generic;
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
        
        /// <summary>
        /// 生成三次贝塞尔曲线
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="control1">控制点1</param>
        /// <param name="control2">控制点2</param>
        /// <param name="end">终点</param>
        /// <param name="segmentCount">路径点数量</param>
        /// <returns></returns>
        public static List<Vector3> GenerateCubicBezier(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, int segmentCount)
        {
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i <= segmentCount; i++)
            {
                float t = i / (float)segmentCount;
                points.Add(Mathf.Pow(1 - t, 3) * start +
                           3 * Mathf.Pow(1 - t, 2) * t * control1 +
                           3 * (1 - t) * Mathf.Pow(t, 2) * control2 +
                           Mathf.Pow(t, 3) * end);
            }

            return points;
        }
    }
}