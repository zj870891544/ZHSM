using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class DefenseTower : TargetableObject
    {
        private DefenseTowerData m_TowerData;
        private NetworkDefenseTower m_NetworkTower;
        private PlayersInAreaDetection m_PlayersInAreaDetection;
        private bool m_IsOccupied;
        private float m_OccupyProgress = 0.0f;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_PlayersInAreaDetection = gameObject.GetComponent<PlayersInAreaDetection>();
            m_PlayersInAreaDetection.SetActivate(true);
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            m_TowerData = userData as DefenseTowerData;
            if (m_TowerData == null)
            {
                Log.Error("DefenseTower is null.");
                return;
            }

            m_IsOccupied = false;
            m_OccupyProgress = 0.0f;

            m_NetworkTower = GetComponent<NetworkDefenseTower>();
            m_NetworkTower.SetOccupyInfo(m_IsOccupied, m_OccupyProgress);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (m_IsOccupied) return;

            if (m_PlayersInAreaDetection.IsAllPlayersTrigger)
            {
                m_OccupyProgress += elapseSeconds * m_TowerData.OccupySpeed;
                if (m_OccupyProgress >= 1.0f)
                {
                    m_OccupyProgress = 1.0f;
                    m_IsOccupied = true;
                }

                m_NetworkTower.SetOccupyInfo(m_IsOccupied, m_OccupyProgress);
            }
        }
    }
}