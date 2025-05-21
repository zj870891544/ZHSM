using Mirror;
using Sirenix.OdinInspector;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ZHSM
{
    public class NetworkGrenadeLauncher : NetworkWeapon
    {
        [Header("手雷发射器设置")]
        [SerializeField] private Transform launchPoint;
        [SerializeField] private float launchForce = 15f;
        [SerializeField] private float reloadTime = 2.0f;
        [SerializeField] private PXR_Input.VibrateType vibrateController = PXR_Input.VibrateType.RightController;

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
        private float reloadTimer = 0f;

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

            base.Start();
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

            // 如果有副按键，可以用来切换手雷类型
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

            // 播放发射效果和声音
            if (isServer)
            {
                FireGrenade();
            }

            // 本地发送网络命令
            CmdFireGrenade((int)currentGrenadeType);

            // 本地进入装弹状态
            canFire = false;
            reloadTimer = 0f;
        }

        [Command(requiresAuthority = false)]
        public void CmdFireGrenade(int grenadeType, NetworkConnectionToClient sender = null)
        {
            if (!canFire)
                return;

            // 服务器处理发射
            if (!isOwned)
            {
                currentGrenadeType = (GrenadeType)grenadeType;
                FireGrenade();
            }

            // 广播给其他客户端
            RpcOtherClientsFire(grenadeType);

            // 服务器设置装弹冷却
            canFire = false;
            reloadTimer = 0f;
        }

        [ClientRpc]
        public void RpcOtherClientsFire(int grenadeType)
        {
            // 其他客户端执行，本地和服务器已执行过，忽略
            if (isOwned || isServer) return;

            currentGrenadeType = (GrenadeType)grenadeType;
            PlayFireEffects();
        }

        [Server]
        private void FireGrenade()
        {
            // 获取当前手雷类型对应的索引
            int index = (int)currentGrenadeType;

            // 播放发射效果和声音
            PlayFireEffects();

            // 创建手雷实体
            int grenadeEntityId = GameEntry.Entity.GenerateSerialId();

            // 准备手雷数据
            GrenadeData grenadeData = new GrenadeData(grenadeEntityId,
                                                      GetGrenadeEntityTypeId(),
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
                ExplosionSoundId = weaponCfg.HitEffectId //使用武器配置中的音效ID
            };

            // 显示手雷实体
            GameEntry.Entity.ShowGrenade(grenadeData);

            // 获取创建的手雷实体，设置发射方向和速度
            NetworkGrenade networkGrenade = null;

            //寻找刚刚创建的手雷
            var entities = FindObjectsOfType<NetworkGrenade>();
            foreach (var entity in entities)
            {
                if (!entity.isClient) // 服务器创建的实体
                {
                    networkGrenade = entity;
                    break;
                }
            }

            // 如果找到了手雷实体，发射它
            if (networkGrenade != null)
            {
                networkGrenade.Launch(launchPoint.forward, launchForce);
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

        // 获取手雷实体类型ID
        private int GetGrenadeEntityTypeId()
        {
            // 这里应根据配置表获取对应类型的EntityId
            // 作为示例，这里返回一个固定值，实际项目中应从配置中读取
            return 2001; // 假设2001是手雷实体的ID
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
            // 可以在这里添加切换类型的视觉/音效反馈
        }
    }
}