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
            
            GameEntry.MultiPlayer.FinishLoadScene();

            Debug.Log("ProcedureMenu Enter >>> ");
        }
    }
}