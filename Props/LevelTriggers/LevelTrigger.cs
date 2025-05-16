using GameFramework.Event;
using Mirror;
using UnityEngine;

namespace ZHSM
{
    public abstract class LevelTrigger : NetworkBehaviour
    {
        [Header("Trigger")]
        [SerializeField] protected string eventName;
        [SerializeField] [ReadOnly] protected bool m_IsActivated = false;
        [SerializeField] [ReadOnly] protected bool m_Triggered = false;

        protected virtual void Start()
        {
            m_IsActivated = false;
            m_Triggered = false;
        }
        
        private void OnEnable()
        {
            GameEntry.Event.Subscribe(LevelEventActivateEventArgs.EventId, OnLevelEventActivate);
        }

        private void OnDisable()
        {
            GameEntry.Event.Unsubscribe(LevelEventActivateEventArgs.EventId, OnLevelEventActivate);
        }

        private void OnLevelEventActivate(object sender, GameEventArgs e)
        {
            LevelEventActivateEventArgs ne = e as LevelEventActivateEventArgs;
            if (ne == null || ne.EventName != eventName) return;
            
            OnActivate();
        }

        protected virtual void OnActivate()
        {
            m_IsActivated = true;
        }

        protected void TriggerEvent()
        {
            if (m_Triggered) return;
            Debug.Log($"level trigger >>> {eventName}");

            m_Triggered = true;
            GameEntry.Event.Fire(this, LevelEventTriggerEventArgs.Create(eventName));
        }
    }
}