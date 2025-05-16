using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ZHSM
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public abstract class XRPlaceItem : MonoBehaviour
    {
        public XRPlaceSelector targetSelector;
        
        protected abstract Transform targetRoot { get; }

        private XRGrabInteractable grabInteractable;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private Transform characterRoot;

        private XRPlaceSelector placementSelector;
        private bool inSelectorArea = false;

        public void SetGrabEnable(bool grabEnable)
        {
            grabInteractable.enabled = grabEnable;
        }

        public void ResetOrigin()
        {
            transform.SetPositionAndRotation(startPosition, startRotation);
        }

        protected virtual void Initialize()
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
            
            grabInteractable = GetComponent<XRGrabInteractable>();
            grabInteractable.selectEntered.AddListener(OnSelectEntered);
            grabInteractable.selectExited.AddListener(OnSelectExited);
        }

        private void Start()
        {
            Initialize();
        }

        // private void Update()
        // {
        //     if (grabInteractable.isSelected)
        //     {
        //         characterRoot.SetPositionAndRotation(
        //             Vector3.Lerp(characterRoot.position, characterSelector.placePoint.position, Time.deltaTime * 30f),
        //             Quaternion.Lerp(characterSelector.placePoint.rotation, characterSelector.placePoint.rotation,
        //                 Time.deltaTime * 30f));
        //     }
        // }

        private void OnSelectEntered(SelectEnterEventArgs arg0)
        {
            if (placementSelector.IsItemSelect(this))
            {
                placementSelector.RemoveItem();
            }
        }

        private void OnSelectExited(SelectExitEventArgs arg0)
        {
            if (!inSelectorArea)
            {
                // 返回
                transform.DOMove(startPosition, 0.3f);
                transform.DORotateQuaternion(startRotation, 0.3f);
            }
            else
            {
                placementSelector.PlaceItem(this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out XRPlaceSelector selector) && selector.Equals(targetSelector))
            {
                inSelectorArea = true;
                placementSelector = selector;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out XRPlaceSelector selector) && selector.Equals(targetSelector))
            {
                inSelectorArea = false;
                placementSelector = null;
            }
        }
    }
}