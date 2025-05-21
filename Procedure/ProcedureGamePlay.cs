using GameFramework.Fsm;
using GameFramework.Procedure;
using Mirror;
using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.cfg;

namespace ZHSM
{
    public class ProcedureGamePlay : ProcedureBase
    {
        private LevelsCfg m_LevelsCfg;
        private GameBase m_CurrentGame;
        
        public override bool UseNativeDialog { get; }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            GameEntry.MultiPlayer.FinishLoadScene();

            int levelId = GameEntry.Level.CurrentLevel;
            Debug.Log("ProcedureGamePlay Enter 关卡：" + levelId);

            if (NetworkServer.active)
            {
                TbLevelsCfg tbLevelsCfg = GameEntry.LubanTable.GetTbLevelsCfg();
                m_LevelsCfg = tbLevelsCfg.GetOrDefault(levelId);
                
                GameMode gameMode = (GameMode)m_LevelsCfg.GameMode;
                switch (gameMode)
                {
                    case GameMode.Survival:
                        m_CurrentGame = Object.Instantiate(GameEntry.BuiltinData.SurvivalTemplate, Vector3.zero,
                            Quaternion.identity);
                        m_CurrentGame.name = "SurvivalGame";
                        break;
                    case GameMode.BeachDefense:
                        m_CurrentGame = Object.Instantiate(GameEntry.BuiltinData.BeachDefenseTemplate, Vector3.zero,
                            Quaternion.identity);
                        m_CurrentGame.name = "BeachDefenseGame";
                        break;
                    case GameMode.BossFight:
                        m_CurrentGame = Object.Instantiate(GameEntry.BuiltinData.BossFightTemplate, Vector3.zero,
                            Quaternion.identity);
                        m_CurrentGame.name = "BossFightGame";
                        break;
                    default:
                        Log.Error($"Unknown GameMode: {gameMode}");
                        return;
                }
                
                m_CurrentGame.Initialize(levelId);
            }
        }

        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            m_CurrentGame?.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            m_CurrentGame?.Shutdown();
        }
    }
}