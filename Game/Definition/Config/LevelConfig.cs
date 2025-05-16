using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ZHSM
{
    public class LevelConfig : ScriptableObject
    {
        [LabelText("出生点坐标")]
        public Vector3 startingPosition = Vector3.zero;
        [LabelText("出生点朝向")]
        public Quaternion startingRotation = Quaternion.identity;
        
        [LabelText("传送门列表")]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 10)]
        [ListDrawerSettings(ShowIndexLabels = true)]
        public List<LevelPortalConfig> portals;
        
#if UNITY_EDITOR
        public virtual void DrawGizmos()
        {
            float handleSize = HandleUtility.GetHandleSize(startingPosition) * 0.5f;
            startingPosition = Handles.FreeMoveHandle(startingPosition, handleSize, Vector3.zero, 
                Handles.RectangleHandleCap);
            Handles.ArrowHandleCap(-1, startingPosition, startingRotation, handleSize, EventType.Repaint);
            
            // 传送门
            if (portals != null)
            {
                Handles.color = Color.green;
                for (int i = 0; i < portals.Count; i++)
                {
                    LevelPortalConfig portalConfig = portals[i];
                    
                    portalConfig.position = Handles.FreeMoveHandle(portalConfig.position, handleSize, Vector3.zero, 
                        Handles.CircleHandleCap);
                    Handles.ArrowHandleCap(i, portalConfig.position, portalConfig.rotation, handleSize, EventType.Repaint);
                    
                    Handles.Label(portalConfig.position, portalConfig.nextLevel.ToString());
                }
            }
        }
#endif
    }
}