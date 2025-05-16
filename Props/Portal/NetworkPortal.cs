using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using ZHSM.cfg;

namespace ZHSM
{
    /// <summary>
    /// 传送门
    /// </summary>
    public class NetworkPortal : NetworkBehaviour
    {
        private Animator animator;
        private Collider portalCollider;
        private HashSet<uint> playersInArea = new HashSet<uint>();
        
        private LevelPortalConfig m_PortalConfig;

        public void Initialize(LevelPortalConfig portalConfig)
        {
            m_PortalConfig = portalConfig;
        }
        
        private void Start()
        {
            animator = GetComponent<Animator>();
            portalCollider = GetComponent<Collider>();

            StartCoroutine(OpenDoor());
        }
        
        private IEnumerator OpenDoor()
        {
            animator?.SetTrigger("Open");

            yield return new WaitForSeconds(1);

            portalCollider.enabled = true;
        }
        
        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerTrigger playerTrigger) )
            {
                if (GameEntry.Level.IsPlayerExist(playerTrigger.NetId))
                {
                    playersInArea.Add(playerTrigger.NetId);
                    
                    CheckAllPlayersInArea();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out PlayerTrigger playerTrigger))
            {
                if (GameEntry.Level.IsPlayerExist(playerTrigger.NetId))
                {
                    playersInArea.Remove(playerTrigger.NetId);
                }
            }
        }

        private void CheckAllPlayersInArea()
        {
            if (!GameEntry.Level.IsAllPlayersExist(playersInArea)) return;
            
            // 所有玩家已全部进入此传送门，开始切换关卡
            GameEntry.Level.LoadLevel(m_PortalConfig.nextLevel);
        }
    }
}