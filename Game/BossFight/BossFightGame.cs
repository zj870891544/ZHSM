using UnityEngine;

namespace ZHSM
{
    public class BossFightGame : GameBase
    {
        public override GameMode GameMode => GameMode.BossFight;

        protected override void OnInitializeSuccess(LevelConfig levelConfig)
        {
            base.OnInitializeSuccess(levelConfig);
            
            // GameEntry.Entity.ShowBoss(new BossData(GameEntry.Entity.GenerateSerialId(), 10001));
        }
    }
}