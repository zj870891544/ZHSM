using System.Collections.Generic;
using GameFramework.Fsm;
using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.FSM;

namespace ZHSM
{
    public class BossEntity : TargetableObject
    {
        private static int FSM_SERIAL_ID = 0;
        
        private BossData m_BossData;
        private NetworkBoss m_Network;
        private Animator m_Animator;
        private IFsm<BossEntity> m_Fsm;
        private int m_HpStepIndex = -1;
        private int m_SkillIndex = 0;

        public Vector3 OriginPosition;
        public Quaternion OriginRotation;
        public NetworkBoss Network => m_Network;
        public List<SkillStep> SkillSteps => m_BossData.SkillSteps;
        public float HPRatio => m_BossData.HPRatio;
        public List<LevelWaveConfig> WaveConfigs => m_BossData.WaveConfigs;
        public SkillAoeCircle AoeCircleConfig => m_BossData.AoeCircleConfig;
        public SkillAoeSector AoeSectorConfig => m_BossData.AoeSectorConfig;
        public SkillSniper SniperConfig => m_BossData.SniperConfig;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_Animator = GetComponent<Animator>();
            m_Network = GetComponent<NetworkBoss>();
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            m_BossData = userData as BossData;
            if (m_BossData == null)
            {
                Log.Error("BossData is invalid.");
                return;
            }

            OriginPosition = CachedTransform.position;
            OriginRotation = CachedTransform.rotation;
            m_HpStepIndex = -1;
            m_SkillIndex = -1;

            m_Fsm = GameEntry.Fsm.CreateFsm((FSM_SERIAL_ID++).ToString(), this,
                new List<FsmState<BossEntity>> { new BossSummonState(), new BossAoeCircleState(), new BossSniperState(), new BossAoeSectorState() });
            BossSkillType skillType = GetSkillType();
            switch (skillType)
            {
                case BossSkillType.SpawnEnemy:
                    m_Fsm.Start<BossSummonState>();
                    break;
                case BossSkillType.AoeCircle:
                    m_Fsm.Start<BossAoeCircleState>();
                    break;
                case BossSkillType.Sniper:
                    m_Fsm.Start<BossSniperState>();
                    break;
                case BossSkillType.AoeSector:
                    m_Fsm.Start<BossAoeSectorState>();
                    break;
                default:
                    Log.Error($"Unknow skill type: {skillType}");
                    break;
            }
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            
            if(m_Fsm != null) GameEntry.Fsm.DestroyFsm(m_Fsm);
        }

        public BossSkillType GetSkillType()
        {
            for (int i = 0; i < SkillSteps.Count; i++)
            {
                SkillStep skillStep = SkillSteps[i];
                
                if (m_BossData.HPRatio > skillStep.hpRatio && 
                    i > 0 ? m_BossData.HPRatio < SkillSteps[i - 1].hpRatio : true)
                {
                    if (m_HpStepIndex != i)
                    {
                        //切换阶段
                        Debug.Log($"生命值下一个阶段 step:{m_HpStepIndex} skill:{m_SkillIndex}");
                        m_HpStepIndex = i;
                        m_SkillIndex = 0;
                    }
                    else
                    {
                        m_SkillIndex++;
                        if (m_SkillIndex > skillStep.skills.Length)
                            m_SkillIndex = 0;
                        
                        Debug.Log($"下一个技能  step:{m_HpStepIndex} skill:{m_SkillIndex} >>> ");
                    }
                    
                    if (skillStep.randomEnable)
                    {
                        int randomIdx = Random.Range(0, SkillSteps.Count);
                        return skillStep.skills[randomIdx];
                    }
                    
                    return skillStep.skills[m_SkillIndex];
                }
            }

            return BossSkillType.None;
        }

        public Vector3 GetJumpDestination(float targetDistance, float closestToPlayer)
        {
            Vector3 startPosition = CachedTransform.position;
            float closestDist = float.MaxValue;
            NetworkTargetable resultTarget = null;
            foreach (NetworkTargetable targetable in GameEntry.Level.Players)
            {
                if (!IsPointInForwardArea(startPosition, targetDistance, 2, targetable.Position)) continue;
                
                float dist = Vector3.Distance(targetable.Position, startPosition);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    resultTarget = targetable;
                }
            }

            if (!resultTarget) return CachedTransform.position + CachedTransform.forward * targetDistance;
            
            return resultTarget.Position + (startPosition - resultTarget.Position).normalized * closestToPlayer;
        }
        
        private bool IsPointInForwardArea(Vector3 start, float length, float width, Vector3 point)
        {
            Vector3 center = Vector3.Lerp(start, start + CachedTransform.forward.normalized * length, 0.5f);
            Vector3 right = CachedTransform.right;
            Vector3 forward = CachedTransform.forward;
            Vector3 normal = Vector3.up;
            
            float distance = Vector3.Dot(point - center, normal);
            Vector3 projectedPoint = point - distance * normal;
            
            Vector3 local = projectedPoint - center;
            float rightDist = Vector3.Dot(local, right);
            float forwardDist = Vector3.Dot(local, forward);
            return Mathf.Abs(rightDist) <= width / 2 &&
                            Mathf.Abs(forwardDist) <= length / 2;
        }
    }
}