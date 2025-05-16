using Mirror;
using UnityEngine;

namespace ZHSM
{
    /// <summary>
    /// ����������
    /// </summary>
    public class ShieldData : WeaponData
    {
        public ShieldData(int entityId, int typeId,int shieldId,NetworkConnectionToClient connection) : base(entityId, typeId, shieldId, connection)
        {
            IsDefending = false;
            DefenseMultiplier = 0.5f;//�������״̬�£��˺�����50%
        }

        /// <summary>
        /// �Ƿ��ڷ���״̬
        /// </summary>
        public bool IsDefending { get;  set; }

        /// <summary>
        /// ����״̬���˺����ⱶ��(0-1֮�䣬0������ȫ�����˺���1�������˺�����)
        /// </summary>
        public float DefenseMultiplier { get; private set; }

    }
}


