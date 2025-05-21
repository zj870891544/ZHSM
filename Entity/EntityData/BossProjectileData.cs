using UnityEngine;

namespace ZHSM
{
    public class BossProjectileData : EntityData
    {
        private bool m_IsDetermineDamage;
        private float m_Speed;
        private Vector3 m_Destination;
        private float m_DamageRadius;
        private int m_Damage;
        private int m_HitEffect;
        private int m_IndicatorEntityId;

        public BossProjectileData(int entityId, int typeId) : base(entityId, typeId)
        {
        }

        public bool IsDetermineDamage
        {
            get { return m_IsDetermineDamage; }
            set { m_IsDetermineDamage = value; }
        }

        public float Speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }

        public Vector3 Destination
        {
            get { return m_Destination; }
            set { m_Destination = value; }
        }

        public float DamageRadius
        {
            get { return m_DamageRadius; }
            set { m_DamageRadius = value; }
        }

        public int Damage
        {
            get { return m_Damage; }
            set { m_Damage = value; }
        }

        public int HitEffect
        {
            get { return m_HitEffect; }
            set { m_HitEffect = value; }
        }

        public int IndicatorEntityId
        {
            get { return m_IndicatorEntityId; }
            set { m_IndicatorEntityId = value; }
        }
    }
}