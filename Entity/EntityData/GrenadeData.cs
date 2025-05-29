using Mirror;
using UnityEngine;

namespace ZHSM
{
    public enum GrenadeType
    {
        Explosive,   // 爆破手雷
        Fire,        // 火焰手雷
        Ice          // 冰冻手雷
    }
    
    public class GrenadeData : WeaponData
    {
        private static int weaponId;

        public GrenadeData(int entityId, int typeId, GrenadeType grenadeType, int damage, float explosionRadius,
            float effectDuration, bool isServerObject, int weaponId, NetworkConnectionToClient connection) : base(entityId, typeId,weaponId,connection)
        {
            GrenadeType = grenadeType;
            Damage = damage;
            ExplosionRadius = explosionRadius;
            EffectDuration = effectDuration;
            IsServerObject = isServerObject;
            
            // 设置默认抛物线参数
            InitialSpeed = 15f;
            LaunchAngle = 45f;
            ArcHeight = 2f;
        }

        public GrenadeType GrenadeType { get; private set; }
        public int Damage { get; private set; }
        public float ExplosionRadius { get; private set; }
        public float EffectDuration { get; private set; }
        public bool IsServerObject { get; private set; }
        public GameObject Owner { get; set; }
        public int ExplosionEffectId { get; set; }
        public int ExplosionSoundId { get; set; }
        
        // 抛物线参数
        public float InitialSpeed { get; set; }  // 初始发射速度
        public float LaunchAngle { get; set; }   // 发射角度（度数）
        public float ArcHeight { get; set; }     // 抛物线高度
        public Vector3 LaunchDirection { get; set; } // 发射方向
    }
}