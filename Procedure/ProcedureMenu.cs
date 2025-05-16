using GameFramework.Event;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;

namespace ZHSM
{
    public class ProcedureMenu : ProcedureBase
    {
        public override bool UseNativeDialog { get; }
        
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            GameEntry.Network.FinishLoadScene();

            Debug.Log("ProcedureMenu Enter >>> ");
        }

        public void StartGame()
        {
            Debug.Log("ProcedureMenu >>> StartGame");
            
            if (GameEntry.BigSpace.isHost)
            {
                GameEntry.Network.StartHost();
                
                // 进入初始关卡
                GameEntry.Level.LoadLevel(GameEntry.Level.StartLevelId);
            }
            else
            {
                GameEntry.Network.StartClient();
            }
        }
    }
}