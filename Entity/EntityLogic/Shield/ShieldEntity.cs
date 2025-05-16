using Mirror;
using Mirror.SimpleWeb;
using UnityEngine;

namespace ZHSM
{
    public class ShieldEntity : WeaponEntity
    {
        private ShieldData m_ShieldData;
        private NetworkShield m_NetworkShield;
        
        //����ģ�����
        private GameObject m_shieldModel;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            //��ȡ����ģ��
            m_shieldModel = transform.Find("Model").gameObject;
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_ShieldData = userData as ShieldData;
            if (m_ShieldData == null)
            {
                Log.Error("ShieldData is null in ShieldEntity's Show.");
                return;
            }

            if (NetworkServer.active)
                NetworkServer.Spawn(gameObject,m_ShieldData.Connection);

            // ��ȡ����������
            m_NetworkShield = GetComponent<NetworkShield>();
            if (m_NetworkShield == null)
            {
                Log.Error("NetworkShield component not found on ShieldEntity.");
                return;
            }
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);

            if (NetworkServer.active)
                NetworkServer.UnSpawn(gameObject);

        }

        #region ����
        /// <summary>
        /// �������״̬
        /// </summary>
        public void StartDefending()
        {
            if (m_ShieldData != null)
            {
                m_ShieldData.IsDefending = true;

                //֪ͨ����������·���״̬
                if (m_NetworkShield != null)
                {
                    m_NetworkShield.RpcSetDefending(true);
                }
            }
        }
        /// <summary>
        /// �˳�����״̬
        /// </summary>
        public void StopDefending()
        {
            if (m_ShieldData != null)
            {
                m_ShieldData.IsDefending = false;
                //֪ͨ����������·���״̬
                if (m_NetworkShield != null)
                {
                    m_NetworkShield.RpcSetDefending(false);
                }
            }
        }

        //�����������˺�
        public float CalculateDefendingDamage(float originalDamage)
        {
            if (m_ShieldData != null && m_ShieldData.IsDefending)
            {
                return originalDamage * m_ShieldData.DefenseMultiplier;
            }
            return originalDamage;
        }


        #endregion
    }
}



