using System;
using Sirenix.OdinInspector;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ZHSM
{
    public class XRRig : MonoBehaviour
    {
        public static XRRig instance;
        
        [Title("XR Components")]
        public XRInteractionManager interactionManager;
        public XROrigin xrOrigin;
        public XRDirectInteractor leftInteractor;
        public XRDirectInteractor rightInteractor;

        [Title("XR Controllers")]
        public ActionBasedController leftHandController;
        public ActionBasedController rightHandController;

        [Title("XR Rig")]
        public Transform headTransform;
        public Transform lHandTransform;
        public Transform rHandTransform;

        private Vector3 m_StandPosition;
        
        public void SelectLeftTarget(GameObject target)
        {
            XRGrabInteractable interactable = target.GetComponent<XRGrabInteractable>();
            if (!interactable) return;
            
            interactionManager.SelectEnter(leftInteractor, interactable);

        }

        public void SelectRightTarget(GameObject target)
        {
            XRGrabInteractable interactable = target.GetComponent<XRGrabInteractable>();
            if (!interactable) return;
            
            interactionManager.SelectEnter(rightInteractor, interactable);
        }

        /// <summary>
        /// 瞬移（中心位置不移动，适合大空间）
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void TeleportTo(Vector3 position, Quaternion rotation)
        {
            xrOrigin.transform.SetPositionAndRotation(position, rotation);
        }

        /// <summary>
        /// 瞬移（大空间方案会错位）
        /// </summary>
        /// <param name="request"></param>
        public void TeleportTo(TeleportRequest request)
        {
            switch (request.matchOrientation)
            {
                case MatchOrientation.WorldSpaceUp:
                    xrOrigin.MatchOriginUp(Vector3.up);
                    break;
                case MatchOrientation.TargetUp:
                    xrOrigin.MatchOriginUp(request.destinationRotation * Vector3.up);
                    break;
                case MatchOrientation.TargetUpAndForward:
                    xrOrigin.MatchOriginUpCameraForward(request.destinationRotation * Vector3.up, request.destinationRotation * Vector3.forward);
                    break;
                case MatchOrientation.None:
                    // Change nothing. Maintain current origin rotation.
                    break;
                default:
                    break;
            }

            var cameraDestination = request.destinationPosition +
                                    xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
            xrOrigin.MoveCameraToWorldLocation(cameraDestination);
        }

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            m_StandPosition.x = headTransform.localPosition.x;
            m_StandPosition.y = transform.localPosition.y;
            m_StandPosition.z = headTransform.localPosition.z;
            
            GameEntry.BigSpace.UpdatePlayerInfo(m_StandPosition, headTransform.rotation);
        }
    }
}