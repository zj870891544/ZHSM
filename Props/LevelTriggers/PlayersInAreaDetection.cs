using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZHSM
{
    public class PlayersInAreaDetection : MonoBehaviour
    {
        private bool m_IsActivate = false;
        private HashSet<uint> m_PlayersInArea = new HashSet<uint>();
        
        private bool m_IsAllPlayersTrigger = false;
        public bool IsAllPlayersTrigger
        {
            get => m_IsAllPlayersTrigger;
            private set
            {
                if (m_IsAllPlayersTrigger != value)
                {
                    m_IsAllPlayersTrigger = value;
                    
                    AllPlayersTriggerChanged?.Invoke(m_IsAllPlayersTrigger);
                }
            }
        }

        public Action<bool> AllPlayersTriggerChanged;

        public void SetActivate(bool isActivate)
        {
            m_IsActivate = isActivate;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!m_IsActivate) return;
            
            if (other.TryGetComponent(out PlayerTrigger playerTrigger))
            {
                if (GameEntry.Level.IsPlayerExist(playerTrigger.NetId))
                {
                    m_PlayersInArea.Add(playerTrigger.NetId);
                    CheckAllPlayersInArea();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!m_IsActivate) return;

            if (other.TryGetComponent(out PlayerTrigger playerTrigger))
            {
                if (GameEntry.Level.IsPlayerExist(playerTrigger.NetId))
                {
                    m_PlayersInArea.Remove(playerTrigger.NetId);
                    
                    IsAllPlayersTrigger = false;
                }
            }
        }

        private void CheckAllPlayersInArea()
        {
            if (GameEntry.Level.IsAllPlayersExist(m_PlayersInArea))
            {
                IsAllPlayersTrigger = true;
            }
        }
    }
}