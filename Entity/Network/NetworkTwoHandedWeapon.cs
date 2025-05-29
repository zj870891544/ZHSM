using Mirror;
using Sirenix.OdinInspector;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ZHSM
{
    /// <summary>
    /// 双手持握武器基类
    /// </summary>
    public class NetworkTwoHandedWeapon : NetworkWeapon
    {
        [Header("双手持握设置")]
        [SerializeField] protected Transform secondaryGrabPoint; // 第二个抓取点
        [SerializeField] protected float stabilityBonus = 0.3f; // 双手持握时的稳定性提升
        [SerializeField] protected float recoilReduction = 0.5f; // 双手持握时的后坐力减少
        
        [Header("双手持握状态")]
        [SerializeField] protected bool isSecondHandGrabbing = false; // 是否被第二只手抓取
        
        // 第二只手的控制器
        protected ActionBasedController secondaryController;
        protected XRGrabInteractable secondaryGrabInteractable;
        
        protected override void Start()
        {
            base.Start();
            
            if (isOwned)
            {
                // 初始化第二个抓取点的交互组件
                if (secondaryGrabPoint != null)
                {
                    secondaryGrabInteractable = secondaryGrabPoint.GetComponent<XRGrabInteractable>();
                    if (secondaryGrabInteractable == null)
                    {
                        secondaryGrabInteractable = secondaryGrabPoint.gameObject.AddComponent<XRGrabInteractable>();
                    }
                    
                    // 设置第二个抓取点的交互属性
                    secondaryGrabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
                    secondaryGrabInteractable.throwOnDetach = false;
                    
                    // 添加事件监听
                    secondaryGrabInteractable.selectEntered.AddListener(OnSecondaryGrabbed);
                    secondaryGrabInteractable.selectExited.AddListener(OnSecondaryReleased);
                }
            }
        }
        
        // protected override void OnDestroy()
        // {
        //     base.OnDestroy();
        //     
        //     // 移除事件监听
        //     if (secondaryGrabInteractable != null)
        //     {
        //         secondaryGrabInteractable.selectEntered.RemoveListener(OnSecondaryGrabbed);
        //         secondaryGrabInteractable.selectExited.RemoveListener(OnSecondaryReleased);
        //     }
        // }
        
        /// <summary>
        /// 第二个抓取点被抓取时调用
        /// </summary>
        protected virtual void OnSecondaryGrabbed(SelectEnterEventArgs args)
        {
            if (!isOwned) return;
            
            isSecondHandGrabbing = true;
            secondaryController = args.interactorObject.transform.parent.GetComponent<ActionBasedController>();
            
            // 发送震动反馈
            if (hapticEnabled)
            {
                PXR_Input.SendHapticImpulse(PXR_Input.VibrateType.LeftController, amplitude * 0.5f, duration / 2, frequency);
            }
            
            // 通知服务器第二只手已抓取
            CmdSetSecondHandGrabbing(true);
            
            Debug.Log("第二只手已抓取武器");
        }
        
        /// <summary>
        /// 第二个抓取点被释放时调用
        /// </summary>
        protected virtual void OnSecondaryReleased(SelectExitEventArgs args)
        {
            if (!isOwned) return;
            
            isSecondHandGrabbing = false;
            secondaryController = null;
            
            // 通知服务器第二只手已释放
            CmdSetSecondHandGrabbing(false);
            
            Debug.Log("第二只手已释放武器");
        }
        
        [Command(requiresAuthority = false)]
        protected virtual void CmdSetSecondHandGrabbing(bool isGrabbing, NetworkConnectionToClient sender = null)
        {
            isSecondHandGrabbing = isGrabbing;
            RpcSetSecondHandGrabbing(isGrabbing);
        }
        
        [ClientRpc]
        protected virtual void RpcSetSecondHandGrabbing(bool isGrabbing)
        {
            // 本地客户端已经设置过，不需要再次设置
            if (isOwned) return;
            
            isSecondHandGrabbing = isGrabbing;
        }
        
        /// <summary>
        /// 获取当前武器的稳定性系数
        /// </summary>
        protected float GetStabilityFactor()
        {
            return isSecondHandGrabbing ? (1.0f + stabilityBonus) : 1.0f;
        }
        
        /// <summary>
        /// 获取当前武器的后坐力系数
        /// </summary>
        protected float GetRecoilFactor()
        {
            return isSecondHandGrabbing ? (1.0f - recoilReduction) : 1.0f;
        }
        
        /// <summary>
        /// 应用后坐力效果
        /// </summary>
        protected virtual void ApplyRecoil(float strength)
        {
            if (!isOwned) return;
            
            // 计算实际后坐力
            float actualRecoil = strength * GetRecoilFactor();
            
            // 应用后坐力效果，例如摄像机抖动或控制器震动
            if (hapticEnabled && selectController != null)
            {
                PXR_Input.SendHapticImpulse(
                    handType == HandType.Right ? PXR_Input.VibrateType.RightController : PXR_Input.VibrateType.LeftController, 
                    amplitude * actualRecoil, 
                    duration, 
                    frequency);
                
                // 如果双手持握，给第二只手也发送震动
                if (isSecondHandGrabbing && secondaryController != null)
                {
                    PXR_Input.SendHapticImpulse(
                        handType == HandType.Right ? PXR_Input.VibrateType.LeftController : PXR_Input.VibrateType.RightController, 
                        amplitude * actualRecoil * 0.7f, 
                        duration, 
                        frequency);
                }
            }
        }
        
        /// <summary>
        /// 计算射击精度
        /// </summary>
        protected virtual float CalculateAccuracy()
        {
            // 基础精度值
            float baseAccuracy = 1.0f;
            
            // 双手持握提高精度
            if (isSecondHandGrabbing)
            {
                baseAccuracy *= GetStabilityFactor();
            }
            
            return baseAccuracy;
        }
        
        /// <summary>
        /// 计算射击散布角度
        /// </summary>
        protected virtual float CalculateSpreadAngle(float baseSpread)
        {
            // 双手持握减小散布
            if (isSecondHandGrabbing)
            {
                return baseSpread * (1.0f - stabilityBonus);
            }
            
            return baseSpread;
        }
    }
} 