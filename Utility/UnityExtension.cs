using System.Collections.Generic;
using UnityEngine;

namespace ZHSM
{
    public static class UnityExtension
    {
        private static readonly List<Transform> s_CachedTransforms = new List<Transform>();
        
        /// <summary>
        /// 递归设置游戏对象的层次
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="layerMask"></param>
        public static void SetLayerMaskRecursively(this GameObject gameObject, LayerMask layerMask)
        {
            int layer = (int)Mathf.Log(layerMask, 2);
            gameObject.GetComponentsInChildren(true, s_CachedTransforms);
            for (int i = 0; i < s_CachedTransforms.Count; i++)
            {
                s_CachedTransforms[i].gameObject.layer = layer;
            }

            s_CachedTransforms.Clear();
        }
    }
}