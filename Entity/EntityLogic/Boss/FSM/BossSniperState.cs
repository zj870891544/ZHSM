using System.Collections.Generic;
using DG.Tweening;
using GameFramework.Fsm;
using MEC;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace ZHSM.FSM
{
    public class BossSniperState : BossState
    {
        protected override void OnEnter(IFsm<BossEntity> fsm)
        {
            base.OnEnter(fsm);
            
            Log.Info("进入狙击 >>> ");
            
            Timing.RunCoroutine(PlaySkill3());
        }

        private IEnumerator<float> PlaySkill3()
        {
            yield return Timing.WaitForSeconds(1.0f);
            
            // 跳跃
            yield return Timing.WaitUntilDone(JumpToDestination());
            
            // 释放技能
            yield return Timing.WaitUntilDone(PlaySkill());
            
            // 跳回
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
            yield return Timing.WaitForSeconds(1.0f);
            
            int randomIndex = Random.Range(0, GameEntry.Level.Players.Count);
            uint playerNetId = GameEntry.Level.Players[randomIndex].netId;
            m_Boss.Network.RpcPlaySkill3(playerNetId);

            yield return Timing.WaitForSeconds(1.35f);
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