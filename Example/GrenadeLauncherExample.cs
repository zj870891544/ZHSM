using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ZHSM.Example
{
    /// <summary>
    /// 手雷发射器使用示例
    /// 这个脚本展示了如何在游戏中集成和使用新的抛物线手雷发射器
    /// </summary>
    public class GrenadeLauncherExample : MonoBehaviour
    {
        [Header("手雷发射器设置")]
        [SerializeField] private NetworkGrenadeLauncher grenadeLauncher;
        [SerializeField] private XRGrabInteractable grabInteractable;
        
        [Header("控制器绑定")]
        [SerializeField] private ActionBasedController rightController;
        
        private bool isPlayerHolding = false;
        
        private void Start()
        {
            // 确保组件存在
            if (grenadeLauncher == null)
                grenadeLauncher = GetComponent<NetworkGrenadeLauncher>();
                
            if (grabInteractable == null)
                grabInteractable = GetComponent<XRGrabInteractable>();
            
            // 绑定抓取事件
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.AddListener(OnWeaponGrabbed);
                grabInteractable.selectExited.AddListener(OnWeaponReleased);
            }
            
            // 获取右手控制器
            if (rightController == null && XRRig.instance != null)
            {
                rightController = XRRig.instance.rightHandController;
            }
        }
        
        private void Update()
        {
            if (isPlayerHolding && rightController != null)
            {
                HandleControllerInput();
            }
        }
        
        private void HandleControllerInput()
        {
            // 检测握把按钮来控制瞄准
            bool gripPressed = false;
            if (rightController.selectActionValue.action != null)
            {
                gripPressed = rightController.selectActionValue.action.ReadValue<float>() > 0.5f;
            }
            
            if (gripPressed && !grenadeLauncher.IsAiming)
            {
                // 开始瞄准
                grenadeLauncher.OnAimActivate();
                Debug.Log("开始瞄准手雷发射器");
            }
            else if (!gripPressed && grenadeLauncher.IsAiming)
            {
                // 停止瞄准
                grenadeLauncher.OnAimDeactivate();
                Debug.Log("停止瞄准手雷发射器");
            }
            
            // 检测菜单按钮来切换手雷类型
            if (rightController.uiPressActionValue.action != null && 
                rightController.uiPressActionValue.action.WasPressedThisFrame())
            {
                grenadeLauncher.SwitchGrenadeType();
                Debug.Log($"切换手雷类型到: {grenadeLauncher.CurrentGrenadeType}");
            }
        }
        
        private void OnWeaponGrabbed(SelectEnterEventArgs args)
        {
            isPlayerHolding = true;
            Debug.Log("玩家抓取了手雷发射器");
            
            // 可以在这里添加UI提示
            ShowControlInstructions(true);
        }
        
        private void OnWeaponReleased(SelectExitEventArgs args)
        {
            isPlayerHolding = false;
            Debug.Log("玩家放下了手雷发射器");
            
            // 确保停止瞄准
            if (grenadeLauncher.IsAiming)
            {
                grenadeLauncher.OnAimDeactivate();
            }
            
            // 隐藏UI提示
            ShowControlInstructions(false);
        }
        
        private void ShowControlInstructions(bool show)
        {
            // 这里可以显示/隐藏控制说明UI
            // 例如：
            // - "握住握把按钮进入瞄准模式"
            // - "使用摇杆调整发射角度"
            // - "按菜单键切换手雷类型"
            // - "按扳机发射手雷"
            
            if (show)
            {
                Debug.Log("显示手雷发射器控制说明");
                // UI显示逻辑
            }
            else
            {
                Debug.Log("隐藏手雷发射器控制说明");
                // UI隐藏逻辑
            }
        }
        
        /// <summary>
        /// 外部调用：强制切换到指定手雷类型
        /// </summary>
        /// <param name="grenadeType">手雷类型</param>
        public void SetGrenadeType(GrenadeType grenadeType)
        {
            if (grenadeLauncher != null)
            {
                grenadeLauncher.CurrentGrenadeType = grenadeType;
                Debug.Log($"手雷类型设置为: {grenadeType}");
            }
        }
        
        /// <summary>
        /// 外部调用：获取当前瞄准状态
        /// </summary>
        /// <returns>是否正在瞄准</returns>
        public bool IsAiming()
        {
            return grenadeLauncher != null && grenadeLauncher.IsAiming;
        }
        
        /// <summary>
        /// 外部调用：获取当前发射角度
        /// </summary>
        /// <returns>发射角度</returns>
        public float GetCurrentLaunchAngle()
        {
            return grenadeLauncher != null ? grenadeLauncher.CurrentLaunchAngle : 0f;
        }
        
        private void OnDestroy()
        {
            // 清理事件监听
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.RemoveListener(OnWeaponGrabbed);
                grabInteractable.selectExited.RemoveListener(OnWeaponReleased);
            }
        }
    }
} 