using UnityEditor;
using UnityEngine;

namespace ZHSM
{
    public class NetworkShotgun : NetworkWeapon
    {
        [SerializeField] private float fireDistance;
        [SerializeField] private float spreadAngle = 30.0f;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!firePoint) return;

            Handles.color = new Color(1f, 0.5f, 0f, 0.3f); // 半透明橙色
            
            Vector3 origin = firePoint.position;
            Vector3 forward = firePoint.forward;
            float angle = spreadAngle / 2f;

            Vector3 discCenter = origin + forward * fireDistance;
            Handles.DrawWireDisc(discCenter, forward, Mathf.Tan(Mathf.Deg2Rad * angle) * fireDistance);
        }
#endif
    }
}