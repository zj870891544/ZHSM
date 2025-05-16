using Mirror;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using ZHSM.cfg;

namespace ZHSM
{
    public class NetworkShield : NetworkWeapon
    {
        [SyncVar(hook = nameof(OnDefendingChanged))][SerializeField]
        protected bool isDefending = false;

        //������ײ�壬�ڷ���״̬������
        [SerializeField] private Collider shieldCollider;

        //����ģ�ͣ������ڷ���״̬�¸ı�λ��/�Ƕ�
        [SerializeField] private Transform shieldModel;

        // �������Ƶ�λ��ƫ�ƺ���ת
        [SerializeField] private Vector3 defendingPositionOffset = new Vector3(0.2f, 0, 0.3f);
        [SerializeField] private Vector3 defendingRotationOffset = new Vector3(0, 30, 0);

        private Vector3 originalPosition;
        private Quaternion originalRotation;

       // protected ShieldCfg ShieldConfig => weaponCfg as ShieldCfg;

        [TargetRpc]
        public void RpcSetDefending(bool defending)
        {
            isDefending = defending;
            UpdateDefendingState(defending);
        }

        private void OnDefendingChanged(bool oldValue, bool newValue)
        {
            UpdateDefendingState(newValue);
        }

        //�̳��Ը����LoadWeaponInfo�������Դ�������ID
        //���ﴦ��������еĳ�ʼ���߼�
        protected void InitializeShield()
        {
            // ��ʼ������
            if (shieldModel != null)
            {
                originalPosition = shieldModel.localPosition;
                originalRotation = shieldModel.localRotation;
            }

            // Ĭ�Ϲر���ײ��
            if (shieldCollider != null)
            {
                shieldCollider.enabled = false;
            }
        }

        private new void Start()
        {
            base.Start();

            if (isOwned)
            {
                isDefending = false;
                // �ҵ����
                if (shieldModel == null)
                {
                    shieldModel = transform.Find("Model") as Transform;
                }

                if (shieldCollider == null && shieldModel != null)
                {
                    shieldCollider = shieldModel.GetComponent<Collider>();
                }

                // ����ԭʼ�任
                if (shieldModel != null)
                {
                    originalPosition = shieldModel.localPosition;
                    originalRotation = shieldModel.localRotation;
                }

                // Ϊ�°�ť��Ӽ����������縱�ֱ��ϵİ�ť��
                // �������ʵ�ʵ�XR��������Ӵ���
            }
        }

        // ���Ǹ����OnSelectExited����
        private new void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            // ��ҷ��¶���ʱ�Ĳ���
            if (isDefending)
            {
                CmdSetDefending(false);
            }
        }


        /// <summary>
        ///���·���״̬���Ӿ��͹���
        /// </summary>
        /// <param name="defending"></param>
        private void UpdateDefendingState(bool defending)
        {
            if (shieldCollider != null)
            {
                shieldCollider.enabled = defending;
            }

            if (shieldModel != null)
            {
                if (defending)
                {
                    //�����������
                    shieldModel.localPosition = originalPosition + defendingPositionOffset;
                    shieldModel.localRotation = originalRotation * Quaternion.Euler(defendingRotationOffset);
                }
                else 
                {
                    //�ָ�ԭʼλ��
                    shieldModel.localPosition = originalPosition;
                    shieldModel.localRotation = originalRotation;
                }
            }
        }

        // ���Ǹ����OnTrigger������ʵ�ֶ������еĴ�����Ϊ
        protected override void OnTrigger()
        {
            // ������ʵ�ֶ�������������Ϊ������У�
        }

        // ���¸�����ʱ�������״̬
        public void OnDefendActivate()
        {
            if (!isDefending)
            {
                CmdSetDefending(true);
            }
        }

        // �ͷŸ�����ʱ�˳�����״̬
        public void OnDefendDeactivate()
        {
            if (isDefending)
            {
                CmdSetDefending(false);
            }
        }

        [Command]
        private void CmdSetDefending(bool defending)
        {
            isDefending = defending;
        }

    }
}

