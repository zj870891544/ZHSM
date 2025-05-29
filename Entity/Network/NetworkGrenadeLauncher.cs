using Mirror;
using Sirenix.OdinInspector;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

namespace ZHSM
{
    public class NetworkGrenadeLauncher : NetworkWeapon
    {
        [Header("手雷发射器设置")]
        [SerializeField] private Transform launchPoint;
        [SerializeField] private float launchForce = 15f;
        [SerializeField] private float reloadTime = 2.0f;
        [SerializeField] private PXR_Input.VibrateType vibrateController = PXR_Input.VibrateType.RightController;
        
        [Header("抛物线轨迹设置")]
        [SerializeField] private LineRenderer trajectoryLine; // 轨迹线渲染器
        [SerializeField] private int trajectoryPoints = 30; // 轨迹点数量
        [SerializeField] private float trajectoryTimeStep = 0.1f; // 轨迹时间步长
        [SerializeField] private float maxTrajectoryTime = 5f; // 最大轨迹时间
        [SerializeField] private LayerMask groundLayerMask = 1; // 地面层级遮罩
        [SerializeField] private GameObject targetIndicator; // 目标指示器
        [SerializeField] private Material trajectoryMaterial; // 轨迹材质
        
        [Header("发射角度控制")]
        [SerializeField] private float minLaunchAngle = 15f; // 最小发射角度
        [SerializeField] private float maxLaunchAngle = 75f; // 最大发射角度
        [SerializeField] private float currentLaunchAngle = 45f; // 当前发射角度
        [SerializeField] private float angleAdjustSpeed = 30f; // 角度调整速度
        
        [Header("弹药设置")]
        [SerializeField] private GrenadeType currentGrenadeType = GrenadeType.Explosive;
        [Tooltip("每种手雷的伤害值")]
        [SerializeField] private int[] grenadeDamage = { 100, 60, 40 };
        [Tooltip("每种手雷的爆炸半径")]
        [SerializeField] private float[] grenadeRadius = { 5f, 4f, 6f };
        [Tooltip("每种手雷的效果持续时间")]
        [SerializeField] private float[] grenadeEffectDuration = { 0f, 5f, 4f };
        [Tooltip("每种手雷的爆炸特效ID")]
        [SerializeField] private int[] grenadeEffectId = { 1001, 1002, 1003 };
        
        [SyncVar] private bool canFire = true;
        [SyncVar] private bool isAiming = false; // 是否正在瞄准
        private float reloadTimer = 0f;
        
        // 轨迹计算相关
        private Vector3[] trajectoryPoints_Array;
        private Vector3 predictedLandingPoint;
        private bool isTrajectoryValid = false;

        // 公共属性，供外部访问
        public bool IsAiming => isAiming;
        public float CurrentLaunchAngle => currentLaunchAngle;
        public GrenadeType CurrentGrenadeType 
        { 
            get => currentGrenadeType; 
            set 
            { 
                currentGrenadeType = value;
                if (isAiming)
                {
                    UpdateTrajectoryVisualization();
                }
            } 
        }

        [Button]
        public void DebugFire()
        {
            OnTrigger();
        }
        
        [Button]
        public void DebugSwitchGrenadeType()
        {
            // 循环切换手雷类型
            currentGrenadeType = (GrenadeType)(((int)currentGrenadeType + 1) % 3);
            UpdateTrajectoryVisualization();
        }

        [Button]
        public void DebugToggleAiming()
        {
            isAiming = !isAiming;
            CmdSetAiming(isAiming);
        }

        protected override void Start()
        {
            // 设置为右手武器
            handType = HandType.Right;
            
            // 如果没有设置发射点，使用枪口点
            if (launchPoint == null)
            {
                launchPoint = firePoint;
            }
            
            // 初始化轨迹线渲染器
            InitializeTrajectoryLine();
            
            // 初始化目标指示器
            InitializeTargetIndicator();
            
            base.Start();
        }
        
        private void InitializeTrajectoryLine()
        {
            if (trajectoryLine == null)
            {
                // 创建轨迹线渲染器
                GameObject trajectoryObj = new GameObject("TrajectoryLine");
                trajectoryObj.transform.SetParent(transform);
                trajectoryLine = trajectoryObj.AddComponent<LineRenderer>();
            }
            
            // 配置轨迹线
            trajectoryLine.material = trajectoryMaterial;
            trajectoryLine.startWidth = 0.02f;
            trajectoryLine.endWidth = 0.02f;
            trajectoryLine.positionCount = trajectoryPoints;
            trajectoryLine.useWorldSpace = true;
            trajectoryLine.enabled = false;
            
            // 初始化轨迹点数组
            trajectoryPoints_Array = new Vector3[trajectoryPoints];
        }
        
        private void InitializeTargetIndicator()
        {
            if (targetIndicator == null)
            {
                // 创建简单的目标指示器
                targetIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                targetIndicator.transform.localScale = Vector3.one * 0.3f;
                targetIndicator.name = "TargetIndicator";
                
                // 设置材质颜色
                Renderer renderer = targetIndicator.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.red;
                }
                
                // 移除碰撞器
                Collider collider = targetIndicator.GetComponent<Collider>();
                if (collider != null)
                {
                    Destroy(collider);
                }
            }
            
            targetIndicator.SetActive(false);
        }
        
        protected override void Update()
        {
            base.Update();
            
            // 处理装弹冷却
            if (!canFire && isServer)
            {
                reloadTimer += Time.deltaTime;
                if (reloadTimer >= reloadTime)
                {
                    canFire = true;
                    reloadTimer = 0f;
                }
            }
            
            // 处理瞄准状态下的轨迹显示
            if (isOwned && isAiming)
            {
                HandleAimingInput();
                UpdateTrajectoryVisualization();
            }
            else if (isOwned)
            {
                // 不在瞄准状态时隐藏轨迹
                HideTrajectory();
            }
        }
        
        private void HandleAimingInput()
        {
            // 使用右手控制器的摇杆来调整发射角度
            if (selectController != null)
            {
                Vector2 thumbstickInput = Vector2.zero;
                
                // 获取摇杆输入（根据您的XR系统调整）
                if (selectController.rotateAnchorAction != null && selectController.rotateAnchorAction.action != null)
                {
                    thumbstickInput = selectController.rotateAnchorAction.action.ReadValue<Vector2>();
                }
                
                // 调整发射角度
                if (Mathf.Abs(thumbstickInput.y) > 0.1f)
                {
                    currentLaunchAngle += thumbstickInput.y * angleAdjustSpeed * Time.deltaTime;
                    currentLaunchAngle = Mathf.Clamp(currentLaunchAngle, minLaunchAngle, maxLaunchAngle);
                }
                
                // 使用UI按钮切换手雷类型 - 使用正确的属性名称
                if (selectController.uiPressActionValue != null && selectController.uiPressActionValue.action != null && 
                    selectController.uiPressActionValue.action.WasPressedThisFrame())
                {
                    SwitchGrenadeType();
                }
            }
        }
        
        private void UpdateTrajectoryVisualization()
        {
            if (!isAiming || launchPoint == null)
            {
                HideTrajectory();
                return;
            }
            
            // 计算发射方向
            Vector3 launchDirection = CalculateLaunchDirection();
            
            // 计算轨迹点
            CalculateTrajectoryPoints(launchPoint.position, launchDirection * launchForce);
            
            // 更新轨迹线显示
            UpdateTrajectoryLine();
            
            // 更新目标指示器
            UpdateTargetIndicator();
        }
        
        private Vector3 CalculateLaunchDirection()
        {
            // 获取发射器的前方向
            Vector3 forward = launchPoint.forward;
            
            // 计算发射角度对应的方向
            Vector3 right = launchPoint.right;
            Vector3 up = Vector3.up;
            
            // 在垂直平面内旋转
            float angleInRadians = currentLaunchAngle * Mathf.Deg2Rad;
            Vector3 direction = forward * Mathf.Cos(angleInRadians) + up * Mathf.Sin(angleInRadians);
            
            return direction.normalized;
        }
        
        private void CalculateTrajectoryPoints(Vector3 startPos, Vector3 startVelocity)
        {
            isTrajectoryValid = false;
            predictedLandingPoint = startPos;
            
            for (int i = 0; i < trajectoryPoints; i++)
            {
                float time = i * trajectoryTimeStep;
                if (time > maxTrajectoryTime) break;
                
                // 计算抛物线轨迹点
                Vector3 point = startPos + startVelocity * time + 0.5f * Physics.gravity * time * time;
                trajectoryPoints_Array[i] = point;
                
                // 检测是否碰撞到地面
                if (i > 0)
                {
                    Vector3 prevPoint = trajectoryPoints_Array[i - 1];
                    if (Physics.Linecast(prevPoint, point, out RaycastHit hit, groundLayerMask))
                    {
                        // 找到落点
                        trajectoryPoints_Array[i] = hit.point;
                        predictedLandingPoint = hit.point;
                        isTrajectoryValid = true;
                        
                        // 截断轨迹
                        for (int j = i + 1; j < trajectoryPoints; j++)
                        {
                            trajectoryPoints_Array[j] = hit.point;
                        }
                        break;
                    }
                }
            }
        }
        
        private void UpdateTrajectoryLine()
        {
            if (trajectoryLine != null)
            {
                trajectoryLine.enabled = true;
                trajectoryLine.positionCount = trajectoryPoints;
                trajectoryLine.SetPositions(trajectoryPoints_Array);
                
                // 根据手雷类型设置轨迹颜色
                Color trajectoryColor = GetGrenadeTypeColor();
                trajectoryLine.material.color = trajectoryColor;
            }
        }
        
        private void UpdateTargetIndicator()
        {
            if (targetIndicator != null && isTrajectoryValid)
            {
                targetIndicator.SetActive(true);
                targetIndicator.transform.position = predictedLandingPoint;
                
                // 根据手雷类型设置指示器颜色和大小
                Renderer renderer = targetIndicator.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = GetGrenadeTypeColor();
                }
                
                // 根据爆炸半径调整指示器大小
                int index = (int)currentGrenadeType;
                float radius = grenadeRadius[index];
                targetIndicator.transform.localScale = Vector3.one * (radius * 0.1f);
            }
            else if (targetIndicator != null)
            {
                targetIndicator.SetActive(false);
            }
        }
        
        private Color GetGrenadeTypeColor()
        {
            switch (currentGrenadeType)
            {
                case GrenadeType.Explosive:
                    return Color.red;
                case GrenadeType.Fire:
                    return new Color(1.0f, 0.5f, 0.0f); // 橙色
                case GrenadeType.Ice:
                    return Color.cyan;
                default:
                    return Color.white;
            }
        }
        
        private void HideTrajectory()
        {
            if (trajectoryLine != null)
            {
                trajectoryLine.enabled = false;
            }
            
            if (targetIndicator != null)
            {
                targetIndicator.SetActive(false);
            }
        }

        protected override void OnTrigger()
        {
            base.OnTrigger();

            if (!isOwned)
            {
                Debug.LogWarning("Weapon is not owner.");
                return;
            }

            if (!canFire)
            {
                return;
            }

            // 震动反馈
            if (hapticEnabled)
            {
                PXR_Input.SendHapticImpulse(vibrateController,
                    amplitude * 1.2f, // 增强震动
                    duration,
                    frequency);
            }

            // 计算发射参数
            Vector3 launchDirection = CalculateLaunchDirection();
            Vector3 launchVelocity = launchDirection * launchForce;

            // 播放发射效果和声音
            if (isServer)
            {
                FireGrenade(launchVelocity);
            }
            
            // 本地发送网络命令
            CmdFireGrenade((int)currentGrenadeType, launchVelocity);
            
            // 本地进入装弹状态
            canFire = false;
            reloadTimer = 0f;
            
            // 发射后隐藏轨迹
            HideTrajectory();
        }
        
        // 瞄准控制方法
        public void OnAimActivate()
        {
            if (isOwned)
            {
                isAiming = true;
                CmdSetAiming(true);
            }
        }

        public void OnAimDeactivate()
        {
            if (isOwned)
            {
                isAiming = false;
                CmdSetAiming(false);
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdSetAiming(bool aiming, NetworkConnectionToClient sender = null)
        {
            isAiming = aiming;
        }
        
        [Command(requiresAuthority = false)]
        public void CmdFireGrenade(int grenadeType, Vector3 launchVelocity, NetworkConnectionToClient sender = null)
        {
            if (!canFire)
                return;
            
            // 服务器处理发射
            if (!isOwned)
            {
                currentGrenadeType = (GrenadeType)grenadeType;
                FireGrenade(launchVelocity);
            }
            
            // 广播给其他客户端
            RpcOtherClientsFire(grenadeType, launchVelocity);
            
            // 服务器设置装弹冷却
            canFire = false;
            reloadTimer = 0f;
        }
        
        [ClientRpc]
        public void RpcOtherClientsFire(int grenadeType, Vector3 launchVelocity)
        {
            // 其他客户端执行，本地和服务器已执行过，忽略
            if (isOwned || isServer) return;
            
            currentGrenadeType = (GrenadeType)grenadeType;
            PlayFireEffects();
        }
        
        [Server]
        private void FireGrenade(Vector3 launchVelocity)
        {
            // 获取当前手雷类型对应的索引
            int index = (int)currentGrenadeType;
            
            // 播放发射效果和声音
            PlayFireEffects();
            
            // 创建手雷实体
            int grenadeEntityId = GameEntry.Entity.GenerateSerialId();
            Debug.LogError("打印下手雷实体ID == " + grenadeEntityId);
            
            // 准备手雷数据
            GrenadeData grenadeData = new GrenadeData(grenadeEntityId, 
                                                      weaponCfg.BulletId,
                                                      currentGrenadeType,
                                                      grenadeDamage[index],
                                                      grenadeRadius[index],
                                                      grenadeEffectDuration[index],
                                                      true,
                                                      weaponId,
                                                      connectionToClient) 
            {
                Position = launchPoint.position,
                Rotation = launchPoint.rotation,
                Owner = gameObject,
                ExplosionEffectId = grenadeEffectId[index],
                ExplosionSoundId = weaponCfg.HitEffectId // 使用武器配置中的音效ID
            };

            var data = grenadeData as WeaponData;
            // 显示手雷实体
            //GameEntry.Entity.ShowGrenade(grenadeData);
            GameEntry.Entity.ShowWeapon(data);
            
            // 获取创建的手雷实体，设置发射速度
            NetworkGrenade networkGrenade = null;
            
            // 寻找刚刚创建的手雷
            var entities = FindObjectsOfType<NetworkGrenade>();
            foreach (var entity in entities)
            {
                if (!entity.isClient) // 服务器创建的实体
                {
                    networkGrenade = entity;
                    break;
                }
            }
            
            // 如果找到了手雷实体，按照计算的速度发射它
            if (networkGrenade != null)
            {
                networkGrenade.LaunchWithVelocity(launchVelocity);
            }
        }
        
        private void PlayFireEffects()
        {
            // 播放发射音效
            GameEntry.Sound.PlaySound(weaponCfg.FireSounds, launchPoint.position);
            
            // 播放发射特效
            if (weaponCfg.FlashEffectId > 0)
            {
                GameEntry.Entity.ShowEffect(new EffectData(GameEntry.Entity.GenerateSerialId(), weaponCfg.FlashEffectId)
                {
                    Position = launchPoint.position,
                    Rotation = launchPoint.rotation
                });
            }
        }
        
        // 切换手雷类型的方法（可以绑定到UI按钮或手柄按键）
        public void SwitchGrenadeType()
        {
            if (!isOwned) return;
            
            // 循环切换到下一种类型
            int nextType = ((int)currentGrenadeType + 1) % 3;
            CmdSwitchGrenadeType(nextType);
        }
        
        [Command(requiresAuthority = false)]
        public void CmdSwitchGrenadeType(int newType, NetworkConnectionToClient sender = null)
        {
            currentGrenadeType = (GrenadeType)newType;
            RpcSyncGrenadeType(newType);
        }
        
        [ClientRpc]
        public void RpcSyncGrenadeType(int newType)
        {
            currentGrenadeType = (GrenadeType)newType;
            // 更新轨迹可视化
            if (isAiming)
            {
                UpdateTrajectoryVisualization();
            }
        }

        /// <summary>
        /// 应用配置文件设置
        /// </summary>
        /// <param name="config">手雷发射器配置</param>
        public void ApplyConfig(ZHSM.Config.GrenadeLauncherConfig config)
        {
            if (config == null) return;

            // 应用基础设置
            launchForce = config.launchForce;
            reloadTime = config.reloadTime;

            // 应用轨迹设置
            trajectoryPoints = config.trajectoryPoints;
            trajectoryTimeStep = config.trajectoryTimeStep;
            maxTrajectoryTime = config.maxTrajectoryTime;
            groundLayerMask = config.groundLayerMask;

            // 应用角度控制设置
            minLaunchAngle = config.minLaunchAngle;
            maxLaunchAngle = config.maxLaunchAngle;
            currentLaunchAngle = config.defaultLaunchAngle;
            angleAdjustSpeed = config.angleAdjustSpeed;

            // 应用手雷数据
            grenadeDamage[0] = config.explosiveDamage;
            grenadeDamage[1] = config.fireDamage;
            grenadeDamage[2] = config.iceDamage;

            grenadeRadius[0] = config.explosiveRadius;
            grenadeRadius[1] = config.fireRadius;
            grenadeRadius[2] = config.iceRadius;

            grenadeEffectDuration[0] = 0f;
            grenadeEffectDuration[1] = config.fireEffectDuration;
            grenadeEffectDuration[2] = config.iceEffectDuration;

            grenadeEffectId[0] = config.explosiveEffectId;
            grenadeEffectId[1] = config.fireEffectId;
            grenadeEffectId[2] = config.iceEffectId;

            // 应用震动设置
            hapticEnabled = config.hapticEnabled;
            amplitude = config.amplitude;
            duration = config.duration;
            frequency = config.frequency;

            // 应用轨迹线设置
            if (trajectoryLine != null)
            {
                if (config.trajectoryMaterial != null)
                {
                    trajectoryLine.material = config.trajectoryMaterial;
                }
                trajectoryLine.startWidth = config.trajectoryWidth;
                trajectoryLine.endWidth = config.trajectoryWidth;
            }

            // 重新初始化轨迹点数组
            trajectoryPoints_Array = new Vector3[trajectoryPoints];

            // 应用目标指示器设置
            if (targetIndicator != null && config.targetIndicatorPrefab != null)
            {
                // 如果有自定义预制体，可以替换当前指示器
                // 这里暂时只调整大小
                targetIndicator.transform.localScale = Vector3.one * config.indicatorBaseSize;
            }

            Debug.Log($"已应用配置到手雷发射器: {gameObject.name}");
        }
    }
} 