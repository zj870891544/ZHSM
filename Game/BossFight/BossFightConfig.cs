using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace ZHSM
{
    [System.Serializable]
    public class SkillStep
    {
        [FormerlySerializedAs("healthProgress")] public float hpRatio;
        public BossSkillType[] skills;
        public bool randomEnable;
    }

    public class BossSkillBase
    {
        [Title("跳跃")]
        public float jumpTargetDistance;
        public float jumpDuration;
        public float closestToPlayer;
    }

    /// <summary>
    /// Boss范围攻击（圆形）
    /// </summary>
    [System.Serializable]
    public class SkillAoeCircle : BossSkillBase
    {
        [Title("蓄力")]
        public float chargeDuration;
        public Vector3 areaCenter;
        public float areaRadius;
        public float circleRadius;
        public int randomCircleNum;

        [Title("Fire")] 
        public string firePoint;

        [PropertySpace]
        public int damage;
    }
    
    /// <summary>
    /// Boss范围攻击（扇形）
    /// </summary>
    [System.Serializable]
    public class SkillAoeSector : BossSkillBase
    {
        [Title("发射")]
        public string[] firePoints;
        public float fireAngle;
        public float fireDistance;
        public int fireNum;
        
        [PropertySpace]
        public int damage;
    }

    /// <summary>
    /// Boss狙击技能
    /// </summary>
    [System.Serializable]
    public class SkillSniper : BossSkillBase
    {
        [LabelText("锁定目标时长")]
        public float lockTargetDuration;
        [LabelText("射击结束等待时长")]
        public float fireOverWaitTime;
        
        [PropertySpace]
        public int damage;
    }

    public enum BossSkillType
    {
        None = 0,
        [LabelText("技能1：召唤小兵")]
        SpawnEnemy = 1,
        [LabelText("技能2：AOE圆形攻击")]
        AoeCircle = 2,
        [LabelText("技能3：狙击")]
        Sniper = 3,
        [LabelText("技能4：AOE扇形攻击")]
        AoeSector = 4
    }
    
    [CreateAssetMenu(menuName = "VRUtils/关卡/Boss战", fileName = "level_x")]
    public class BossFightConfig : LevelConfig
    {
        [Title("Boss配置")]
        public int bossId;
        public Vector3 bossPosition;
        public Quaternion bossRotation;
        [LabelText("技能轴列表")]
        public List<SkillStep> skillSteps;

        [Title("技能配置")]
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        [LabelText("技能1：召唤小兵")]
        [ListDrawerSettings(ShowIndexLabels = true, ShowItemCount = true)]
        public List<LevelWaveConfig> waves;
        [LabelText("技能2：AOE圆形攻击")]
        public SkillAoeCircle aoeCircle;
        [LabelText("技能3：狙击")]
        public SkillSniper sniper;
        [LabelText("技能4：AOE扇形攻击")]
        public SkillAoeSector aoeSector;

#if UNITY_EDITOR
        public override void DrawGizmos()
        {
            base.DrawGizmos();
            
            Handles.color = Color.red;
            
            float spacing = aoeCircle.circleRadius * 2f; // 点间距
            for (float x = aoeCircle.areaCenter.x - aoeCircle.areaRadius + aoeCircle.circleRadius; x <= aoeCircle.areaCenter.x + aoeCircle.areaRadius - aoeCircle.circleRadius; x += spacing)
            {
                for (float z = aoeCircle.areaCenter.z - aoeCircle.areaRadius + aoeCircle.circleRadius; z <= aoeCircle.areaCenter.z + aoeCircle.areaRadius - aoeCircle.circleRadius; z += spacing)
                {
                    Vector3 point = new Vector3(x, 0, z);
                    Handles.CircleHandleCap((int)(x*z), point, Quaternion.LookRotation(Vector3.up), aoeCircle.circleRadius, EventType.Repaint);
                }
            }
        }
#endif
    }
}