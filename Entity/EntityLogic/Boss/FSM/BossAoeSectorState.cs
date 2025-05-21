using GameFramework.Fsm;
using UnityGameFramework.Runtime;

namespace ZHSM.FSM
{
    public class BossAoeSectorState : BossState
    {
        protected override void OnEnter(IFsm<BossEntity> fsm)
        {
            base.OnEnter(fsm);
            
            Log.Info("进入扇形AOE >>> ");
        }
    }
}