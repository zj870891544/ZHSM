using System.Collections.Generic;
using DG.Tweening;
using MEC;
using Mirror;
using Sirenix.OdinInspector;
using Telepathy;
using UnityEngine;
using UnityEngine.Animations;

namespace ZHSM
{
    public class NetworkBoss : NetworkTargetable
    {
        [Title("Jump")]
        [SerializeField] private float m_JumpDuration = 0.8f;
        [SerializeField] private Ease m_JumpEase = Ease.Linear;
        [SerializeField] private float m_JumpBezierAngle = 30.0f;
        [SerializeField] private float m_JumpBezierLength = 5;
        [SerializeField] private int m_JumpBezierCount = 30;
        [SerializeField] private int m_JumpGroundEffectId;
        
        [Title("Skill2")]
        [SerializeField] private int m_CircleIndicatorId;
        [SerializeField] private float m_Skill2ChargeDuration = 0.6f;
        [SerializeField] private Transform[] m_Skill2ChargeAttachPoints;
        [SerializeField] private Transform[] m_Skill2FirePoints;
        private List<Vector3> m_Skill2Points;
        private float m_Skill2CircleRadius;
        private List<int> m_Skill2CircleIndicators;
        
        [Title("Skill3")]
        [SerializeField] private LookAtConstraint m_LookAtConstraint;
        [SerializeField] private Transform m_Skill3FirePoint;
        [SerializeField] private BossLaser m_Skill3Laser;
        private NetworkPlayer m_Skill3Target;
        
        private Animator m_Animator;
        
        public override Vector3 Position => transform.position;
        public float JumpDuration => m_JumpDuration;

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }

        [ClientRpc]
        public void RpcPlaySkill3(uint targetPlayerId)
        {
            foreach (NetworkTargetable player in GameEntry.Level.Players)
            {
                if (player.netId == targetPlayerId)
                {
                    m_Skill3Target = player as NetworkPlayer;
                    break;
                }
            }

            if (!m_Skill3Target)
            {
                Log.Error($"Player {targetPlayerId} does not found!");
                return;
            }

            m_LookAtConstraint.constraintActive = false;
            
            ConstraintSource constraintSource = new ConstraintSource();
            constraintSource.sourceTransform = m_Skill3Target.headTransform;
            constraintSource.weight = 1;
            m_LookAtConstraint.AddSource(constraintSource);
            
            m_Skill3Laser.SetTarget(m_Skill3Target.headTransform);
            
            m_Animator.SetTrigger("PlaySkill3");
        }

        private void AnimSkill3Focus()
        {
            m_Skill3Laser.SetVisible(true);
            m_LookAtConstraint.constraintActive = true;
        }

        private void AnimSkill3Fire()
        {
            m_Skill3Laser.SetVisible(false);
            m_LookAtConstraint.constraintActive = false;
            m_LookAtConstraint.RemoveSource(0);
            
            //fire
        }

        [ClientRpc]
        public void RpcPlaySkill2(List<Vector3> points, float radius)
        {
            m_Skill2Points = points;
            m_Skill2CircleRadius = radius;
            
            m_Animator.SetTrigger("PlaySkill2");
        }
        
        /// <summary>
        /// 技能2蓄力
        /// </summary>
        private void AnimSkill2Charge()
        {
            //蓄力
            foreach (Transform chargeAttachPoint in m_Skill2ChargeAttachPoints)
            {
                int chargeEntityId = GameEntry.Entity.GenerateSerialId();
                GameEntry.Entity.ShowEffect(new EffectData(chargeEntityId, 50009)
                {
                    Position = chargeAttachPoint.position,
                    Rotation = chargeAttachPoint.rotation,
                    KeepTime = m_Skill2ChargeDuration,
                    AttachPoint = chargeAttachPoint
                });
            }
            
            //落点预警
            m_Skill2CircleIndicators = new List<int>();
            for (int i = 0; i < m_Skill2Points.Count; i++)
            {
                int entityId = GameEntry.Entity.GenerateSerialId();
                GameEntry.Entity.ShowCircleIndicator(new CircleIndicatorData(entityId, m_CircleIndicatorId)
                {
                    Position = m_Skill2Points[i],
                    Rotation = Quaternion.identity,
                    Radius = m_Skill2CircleRadius
                });
                
                m_Skill2CircleIndicators.Add(entityId);
            }
        }

        private void AnimSkill2Fire()
        {
            foreach (Transform firePoint in m_Skill2FirePoints)
            {
                GameEntry.Entity.ShowEffect(new EffectData(GameEntry.Entity.GenerateSerialId(), 50004)
                {
                    Position = firePoint.position,
                    Rotation = firePoint.rotation
                });
            }
        }

        private void AnimSkill2Projectile()
        {
            if (m_Skill2CircleIndicators == null || m_Skill2CircleIndicators.Count <= 0) return;
            
            foreach (int indicatorId in m_Skill2CircleIndicators)
            {
                UnityGameFramework.Runtime.Entity indicatorEntity = GameEntry.Entity.GetEntity(indicatorId);
                if(indicatorEntity == null) continue;
                
                GameEntry.Entity.ShowBossProjectile(new BossProjectileData(GameEntry.Entity.GenerateSerialId(), 60003)
                {
                    Position = indicatorEntity.transform.position + new Vector3(0.0f, 10.0f, 0.0f),
                    Rotation = Quaternion.identity,
                    Damage = 100,
                    DamageRadius = m_Skill2CircleRadius,
                    Destination = indicatorEntity.transform.position,
                    HitEffect = 50006,
                    IsDetermineDamage = NetworkServer.active,
                    Speed = 20,
                    IndicatorEntityId = indicatorId
                });
            }
        }

        #region Jump
        [ClientRpc]
        public void RpcJumpToTarget(Vector3 start, Vector3 end)
        {
            Timing.RunCoroutine(JumpToTargetCoroutine(start, end));
        }
        
        private IEnumerator<float> JumpToTargetCoroutine(Vector3 start, Vector3 end)
        {
            m_Animator.SetTrigger("JumpTrigger");
            yield return Timing.WaitForSeconds(0.26f);

            List<Vector3> jumpPoints = GenerateJumpPath(start, end);
            transform.DOPath(jumpPoints.ToArray(), m_JumpDuration).SetEase(m_JumpEase);
            yield return Timing.WaitForSeconds(m_JumpDuration);
            
            m_Animator.SetTrigger("JumpGroundTrigger");
            
            // 落地灰尘
            GameEntry.Entity.ShowEffect(new EffectData(GameEntry.Entity.GenerateSerialId(), m_JumpGroundEffectId)
            {
                Position = transform.position,
                Rotation = Quaternion.identity
            });
            
            yield return Timing.WaitForSeconds(0.6f);
        }

        private List<Vector3> GenerateJumpPath(Vector3 start, Vector3 end)
        {
            Vector3 direction = (end - start).normalized;
            Vector3 elevatedDir = Quaternion.AngleAxis(m_JumpBezierAngle, Vector3.Cross(Vector3.down, direction)) * direction;
            Vector3 control1 = start + elevatedDir * m_JumpBezierLength * 0.5f;
            
            direction = (start - end).normalized;
            elevatedDir = Quaternion.AngleAxis(m_JumpBezierAngle, Vector3.Cross(Vector3.down, direction)) * direction;
            Vector3 control2 = end + elevatedDir * m_JumpBezierLength;

            return AIUtility.GenerateCubicBezier(start, control1, control2, end, m_JumpBezierCount);
        }
        #endregion
    }
}