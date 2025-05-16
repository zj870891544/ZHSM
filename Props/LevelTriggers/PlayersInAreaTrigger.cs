using Mirror;
using UnityEngine;

namespace ZHSM
{
    /// <summary>
    /// 所有玩家进入某一个区域触发
    /// </summary>
    [RequireComponent(typeof(PlayersInAreaDetection))]
    public class PlayersInAreaTrigger : LevelTrigger
    {
        [Header("Generate")]
        [SerializeField] private float delayTrigger = 1.0f;
        [SerializeField] private Collider triggerCollider;
        
        private PlayersInAreaDetection playersInAreaDetection;
        private float delayTimer = 0.0f;

        protected override void Start()
        {
            base.Start();

            playersInAreaDetection = GetComponent<PlayersInAreaDetection>();
            playersInAreaDetection.AllPlayersTriggerChanged += allPlayersTrigger =>
            {
                if (!allPlayersTrigger)
                {
                    delayTimer = 0.0f;
                }
            };
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            
            playersInAreaDetection.SetActivate(true);
        }

        private void Update()
        {
            if (!m_IsActivated || m_Triggered) return;
            
            if (playersInAreaDetection.IsAllPlayersTrigger)
            {
                delayTimer += Time.deltaTime;
                if (delayTimer >= delayTrigger)
                {
                    delayTimer = 0.0f;
                    
                    TriggerEvent();
                    playersInAreaDetection.SetActivate(false);
                }
            }
        }
    }
}