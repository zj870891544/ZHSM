using Mirror;
using Mirror.SimpleWeb;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using ZHSM.cfg;

namespace ZHSM
{
    public class NetworkWeapon : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnWeaponIdUpdate))] [SerializeField]
        protected int weaponId;

        [SyncVar(hook = nameof(OnTriggeredChanged))] [SerializeField]
        protected bool isTriggered = false;
        
        [Header("震动反馈")]
        [SerializeField] protected bool hapticEnabled = true;
        [SerializeField] protected float amplitude = 0.5f;
        [SerializeField] protected int duration = 500;
        [SerializeField] protected int frequency = 500;
        
        private bool isSelected = false;
        private XRGrabInteractable grabInteractable;
        private ActionBasedController selectController;
        private InputAction activateValueAction;
        
        protected WeaponCfg weaponCfg;
        protected Transform firePoint;

        [TargetRpc]
        public void RpcSetWeaponId(NetworkConnectionToClient target, int _weaponId)
        {
            weaponId = _weaponId;
            
            LoadWeaponInfo(weaponId);
        }
        
        private void OnWeaponIdUpdate(int oldValue, int newValue)
        {
            LoadWeaponInfo(newValue);
        }

        private void LoadWeaponInfo(int weaponId)
        {
            TbWeaponCfg tbWeaponCfg = GameEntry.LubanTable.GetTbWeaponCfg();
            weaponCfg = tbWeaponCfg.GetOrDefault(weaponId);

            firePoint = transform.Find(weaponCfg.FirePoint);
        }

        protected virtual void Start()
        {
            if (isOwned)
            {
                isSelected = false;
                isTriggered = false;
                
                grabInteractable = GetComponent<XRGrabInteractable>();
                grabInteractable.selectEntered.AddListener(OnSelectEntered);
                grabInteractable.selectExited.AddListener(OnSelectExited);
                grabInteractable.activated.AddListener(OnActivated);
                grabInteractable.deactivated.AddListener(OnDeactivated);
                
                XRRig.instance.SelectRightTarget(gameObject);
            }
        }

        protected virtual void Update()
        {
            if (isOwned && isSelected && activateValueAction != null)
            {
                OnTriggerValueUpdate(activateValueAction.ReadValue<float>());
            }
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            selectController = args.interactorObject.transform.parent.GetComponent<ActionBasedController>();
            activateValueAction = selectController.activateActionValue.action;

            OnTriggerValueUpdate(0.0f);
            
            isSelected = true;
        }
        
        protected virtual void OnSelectExited(SelectExitEventArgs arg0)
        {
            OnTriggerValueUpdate(0.0f);
            
            isSelected = false;
        }

        public void OnActivated(ActivateEventArgs args)
        {
            isTriggered = true;
            
            OnTrigger();
        }
        
        private void OnDeactivated(DeactivateEventArgs arg0)
        {
            isTriggered = false;
        }
        
        protected virtual void OnTrigger()
        {
            
        }

        protected virtual void OnTriggerValueUpdate(float triggerValue)
        {
            
        }

        protected virtual void OnTriggeredChanged(bool oldValue, bool newValue)
        {
            
        }
    }
}