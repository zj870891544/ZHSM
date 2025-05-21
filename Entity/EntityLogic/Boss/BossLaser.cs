using UnityEngine;

namespace ZHSM
{
    public class BossLaser : MonoBehaviour
    {
        private LineRenderer m_LaserLine;
        private Transform m_LaserTarget;
        private bool m_Visible = false;

        private void Awake()
        {
            m_LaserLine = GetComponent<LineRenderer>();
            m_LaserLine.useWorldSpace = true;
            m_LaserLine.enabled = false;
        }

        private void Update()
        {
            if (m_Visible)
            {
                m_LaserLine.SetPosition(0, transform.position);
                if (m_LaserTarget) m_LaserLine.SetPosition(1, m_LaserTarget.position);
            }
        }

        public void SetTarget(Transform target)
        {
            m_LaserTarget = target;
        }

        public void SetVisible(bool visible)
        {
            m_Visible = visible;
            m_LaserLine.enabled = visible;
        }
    }
}