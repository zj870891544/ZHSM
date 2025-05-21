using System.Collections.Generic;
using DG.Tweening;
using GameFramework.Fsm;
using MEC;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace ZHSM.FSM
{
    public class BossAoeCircleState : BossState
    {
        protected override void OnEnter(IFsm<BossEntity> fsm)
        {
            base.OnEnter(fsm);
            
            Log.Info("进入圆形AOE >>> ");
            
            Timing.RunCoroutine(PlaySkill2());
        }

        private IEnumerator<float> PlaySkill2()
        {
            // 跳跃
            yield return Timing.WaitUntilDone(JumpToDestination());
            
            // 释放技能
            yield return Timing.WaitUntilDone(PlaySkill());
            
            //跳回
            yield return Timing.WaitUntilDone(JumpBack());
            
            OnSkillComplete();
        }

        private IEnumerator<float> JumpToDestination()
        {
            yield return Timing.WaitForSeconds(3.0f);

            Log.Info("Jump >>> ");
            m_Boss.Network.RpcJumpToTarget(m_Boss.CachedTransform.position,
                m_Boss.GetJumpDestination(m_Boss.AoeCircleConfig.jumpTargetDistance,
                    m_Boss.AoeCircleConfig.closestToPlayer));
            yield return Timing.WaitForSeconds(m_Boss.Network.JumpDuration + 0.6f);
            
            Log.Info("Jump over >>> ");
        }

        private IEnumerator<float> PlaySkill()
        {
            // 技能前摇
            yield return Timing.WaitForSeconds(1.0f);

            Log.Info("播放技能2 >>> ");
            Vector3 avoidPoint = m_Boss.CachedTransform.position;
            List<Vector3> gridPoints = new List<Vector3>();
            SkillAoeCircle aoeCircle = m_Boss.AoeCircleConfig;
            float spacing = aoeCircle.circleRadius * 2f; // 点间距
            for (float x = aoeCircle.areaCenter.x - aoeCircle.areaRadius + aoeCircle.circleRadius; x <= aoeCircle.areaCenter.x + aoeCircle.areaRadius - aoeCircle.circleRadius; x += spacing)
            {
                for (float z = aoeCircle.areaCenter.z - aoeCircle.areaRadius + aoeCircle.circleRadius; z <= aoeCircle.areaCenter.z + aoeCircle.areaRadius - aoeCircle.circleRadius; z += spacing)
                {
                    Vector3 point = new Vector3(x, 0, z);
                    // 确保点在大圆内部
                    if (Vector2.Distance(point, aoeCircle.areaCenter) + aoeCircle.circleRadius <= aoeCircle.areaRadius)
                    {
                        // 避开boss所在位置
                        if (Vector2.Distance(point, avoidPoint) >= aoeCircle.circleRadius * 2f)
                        {
                            gridPoints.Add(point);
                        }
                    }
                }
            }

            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < aoeCircle.randomCircleNum; i++)
            {
                int randomIdx = Random.Range(0, gridPoints.Count);
                points.Add(gridPoints[randomIdx]);
                
                gridPoints.RemoveAt(randomIdx);
            }
            
            m_Boss.Network.RpcPlaySkill2(points, aoeCircle.circleRadius);

            yield return Timing.WaitForSeconds(5);
        }

        private IEnumerator<float> JumpBack()
        {
            Log.Info("Jump Back >>> ");
            Vector3 offset = m_Boss.OriginPosition - m_Boss.CachedTransform.position;
            offset.y = 0;
            
            m_Boss.CachedAnimator.SetFloat("MoveSpeed", 0.5f);
            m_Boss.CachedTransform.DORotateQuaternion(Quaternion.LookRotation(offset), 0.5f);
            yield return Timing.WaitForSeconds(0.5f);

            m_Boss.CachedAnimator.SetFloat("MoveSpeed", 0.0f);
            m_Boss.Network.RpcJumpToTarget(m_Boss.CachedTransform.position, m_Boss.OriginPosition);
            yield return Timing.WaitForSeconds(m_Boss.Network.JumpDuration + 0.6f);
            
            m_Boss.CachedAnimator.SetFloat("MoveSpeed", 0.5f);
            m_Boss.CachedTransform.DORotateQuaternion(m_Boss.OriginRotation, 0.5f);
            yield return Timing.WaitForSeconds(0.5f);
            m_Boss.CachedAnimator.SetFloat("MoveSpeed", 0.0f);
        }
    }
}