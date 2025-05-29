using UnityEngine;
using Sirenix.OdinInspector;

namespace ZHSM.Config
{
    /// <summary>
    /// 手雷发射器配置文件
    /// 用于统一管理手雷发射器的各种参数设置
    /// </summary>
    [CreateAssetMenu(fileName = "GrenadeLauncherConfig", menuName = "ZHSM/Configs/Grenade Launcher Config")]
    public class GrenadeLauncherConfig : ScriptableObject
    {
        [TabGroup("基础设置")]
        [Header("发射参数")]
        [SerializeField, Range(5f, 50f)] 
        [Tooltip("手雷发射力度")]
        public float launchForce = 15f;
        
        [SerializeField, Range(0.5f, 5f)]
        [Tooltip("装弹时间（秒）")]
        public float reloadTime = 2.0f;
        
        [TabGroup("轨迹设置")]
        [Header("轨迹可视化")]
        [SerializeField, Range(10, 50)]
        [Tooltip("轨迹线点数")]
        public int trajectoryPoints = 30;
        
        [SerializeField, Range(0.05f, 0.2f)]
        [Tooltip("轨迹时间步长")]
        public float trajectoryTimeStep = 0.1f;
        
        [SerializeField, Range(2f, 10f)]
        [Tooltip("最大轨迹时间")]
        public float maxTrajectoryTime = 5f;
        
        [SerializeField]
        [Tooltip("地面检测层级")]
        public LayerMask groundLayerMask = 1;
        
        [TabGroup("角度控制")]
        [Header("发射角度")]
        [SerializeField, Range(5f, 30f)]
        [Tooltip("最小发射角度")]
        public float minLaunchAngle = 15f;
        
        [SerializeField, Range(60f, 90f)]
        [Tooltip("最大发射角度")]
        public float maxLaunchAngle = 75f;
        
        [SerializeField, Range(30f, 60f)]
        [Tooltip("默认发射角度")]
        public float defaultLaunchAngle = 45f;
        
        [SerializeField, Range(10f, 60f)]
        [Tooltip("角度调整速度")]
        public float angleAdjustSpeed = 30f;
        
        [TabGroup("手雷类型")]
        [Header("爆破手雷")]
        [SerializeField, Range(50, 200)]
        public int explosiveDamage = 100;
        
        [SerializeField, Range(3f, 8f)]
        public float explosiveRadius = 5f;
        
        [SerializeField]
        public int explosiveEffectId = 1001;
        
        [TabGroup("手雷类型")]
        [Header("火焰手雷")]
        [SerializeField, Range(30, 100)]
        public int fireDamage = 60;
        
        [SerializeField, Range(2f, 6f)]
        public float fireRadius = 4f;
        
        [SerializeField, Range(3f, 8f)]
        public float fireEffectDuration = 5f;
        
        [SerializeField]
        public int fireEffectId = 1002;
        
        [TabGroup("手雷类型")]
        [Header("冰冻手雷")]
        [SerializeField, Range(20, 80)]
        public int iceDamage = 40;
        
        [SerializeField, Range(4f, 8f)]
        public float iceRadius = 6f;
        
        [SerializeField, Range(2f, 6f)]
        public float iceEffectDuration = 4f;
        
        [SerializeField]
        public int iceEffectId = 1003;
        
        [TabGroup("视觉效果")]
        [Header("轨迹线材质")]
        [SerializeField]
        [Tooltip("轨迹线材质")]
        public Material trajectoryMaterial;
        
        [SerializeField, Range(0.01f, 0.1f)]
        [Tooltip("轨迹线宽度")]
        public float trajectoryWidth = 0.02f;
        
        [TabGroup("视觉效果")]
        [Header("目标指示器")]
        [SerializeField]
        [Tooltip("目标指示器预制体")]
        public GameObject targetIndicatorPrefab;
        
        [SerializeField, Range(0.1f, 1f)]
        [Tooltip("指示器基础大小")]
        public float indicatorBaseSize = 0.3f;
        
        [SerializeField, Range(0.05f, 0.2f)]
        [Tooltip("指示器大小缩放系数")]
        public float indicatorSizeMultiplier = 0.1f;
        
        [TabGroup("震动反馈")]
        [Header("VR震动设置")]
        [SerializeField]
        [Tooltip("是否启用震动反馈")]
        public bool hapticEnabled = true;
        
        [SerializeField, Range(0.1f, 1f)]
        [Tooltip("震动强度")]
        public float amplitude = 0.5f;
        
        [SerializeField, Range(100, 1000)]
        [Tooltip("震动持续时间（毫秒）")]
        public int duration = 500;
        
        [SerializeField, Range(100, 1000)]
        [Tooltip("震动频率")]
        public int frequency = 500;
        
        /// <summary>
        /// 获取指定手雷类型的伤害值
        /// </summary>
        public int GetGrenadeDamage(GrenadeType type)
        {
            return type switch
            {
                GrenadeType.Explosive => explosiveDamage,
                GrenadeType.Fire => fireDamage,
                GrenadeType.Ice => iceDamage,
                _ => explosiveDamage
            };
        }
        
        /// <summary>
        /// 获取指定手雷类型的爆炸半径
        /// </summary>
        public float GetGrenadeRadius(GrenadeType type)
        {
            return type switch
            {
                GrenadeType.Explosive => explosiveRadius,
                GrenadeType.Fire => fireRadius,
                GrenadeType.Ice => iceRadius,
                _ => explosiveRadius
            };
        }
        
        /// <summary>
        /// 获取指定手雷类型的效果持续时间
        /// </summary>
        public float GetGrenadeEffectDuration(GrenadeType type)
        {
            return type switch
            {
                GrenadeType.Explosive => 0f,
                GrenadeType.Fire => fireEffectDuration,
                GrenadeType.Ice => iceEffectDuration,
                _ => 0f
            };
        }
        
        /// <summary>
        /// 获取指定手雷类型的特效ID
        /// </summary>
        public int GetGrenadeEffectId(GrenadeType type)
        {
            return type switch
            {
                GrenadeType.Explosive => explosiveEffectId,
                GrenadeType.Fire => fireEffectId,
                GrenadeType.Ice => iceEffectId,
                _ => explosiveEffectId
            };
        }
        
        /// <summary>
        /// 获取指定手雷类型的颜色
        /// </summary>
        public Color GetGrenadeColor(GrenadeType type)
        {
            return type switch
            {
                GrenadeType.Explosive => Color.red,
                GrenadeType.Fire => new Color(1.0f, 0.5f, 0.0f), // 橙色
                GrenadeType.Ice => Color.cyan,
                _ => Color.white
            };
        }
        
        [Button("应用到所有手雷发射器")]
        [TabGroup("工具")]
        private void ApplyToAllGrenadeLaunchers()
        {
            NetworkGrenadeLauncher[] launchers = FindObjectsOfType<NetworkGrenadeLauncher>();
            foreach (var launcher in launchers)
            {
                launcher.ApplyConfig(this);
            }
            Debug.Log($"已将配置应用到 {launchers.Length} 个手雷发射器");
        }
        
        [Button("重置为默认值")]
        [TabGroup("工具")]
        private void ResetToDefaults()
        {
            launchForce = 15f;
            reloadTime = 2.0f;
            trajectoryPoints = 30;
            trajectoryTimeStep = 0.1f;
            maxTrajectoryTime = 5f;
            minLaunchAngle = 15f;
            maxLaunchAngle = 75f;
            defaultLaunchAngle = 45f;
            angleAdjustSpeed = 30f;
            
            explosiveDamage = 100;
            explosiveRadius = 5f;
            fireDamage = 60;
            fireRadius = 4f;
            fireEffectDuration = 5f;
            iceDamage = 40;
            iceRadius = 6f;
            iceEffectDuration = 4f;
            
            trajectoryWidth = 0.02f;
            indicatorBaseSize = 0.3f;
            indicatorSizeMultiplier = 0.1f;
            
            hapticEnabled = true;
            amplitude = 0.5f;
            duration = 500;
            frequency = 500;
            
            Debug.Log("配置已重置为默认值");
        }
    }
} 