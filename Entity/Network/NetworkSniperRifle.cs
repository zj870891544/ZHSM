using Mirror;
using Sirenix.OdinInspector;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace ZHSM
{
    public class NetworkSniperRifle : NetworkWeapon
    {
        [Header("狙击枪设置")]
        [SerializeField] private float zoomFOV = 20f;  // 瞄准时的FOV
        [SerializeField] private float normalFOV = 60f;  // 正常FOV
        [SerializeField] private float reloadTime = 1.5f;  // 拉栓时间

        [SyncVar(hook = nameof(OnAimingChanged))]
        private bool isAiming = false;  // 是否正在瞄准

        private bool canFire = true;  // 是否可以开火（用于拉栓冷却）
        private float reloadTimer = 0f;  // 拉栓计时器

        private ActionBasedController leftController;  // 左手控制器（用于辅助瞄准）
        private Camera playerCamera;  // 玩家相机（用于修改FOV）

        [Button]
        public void DebugFire()
        {
            OnTrigger();
        }

        [Button]
        public void DebugToggleAim()
        {
            isAiming = !isAiming;
            CmdSetAiming(isAiming);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            // 尝试获取玩家相机
            if (isOwned)
            {
                playerCamera = Camera.main;
                XRRig xrRig = XRRig.instance;
                if (xrRig != null)
                {
                    leftController = xrRig.leftHandController;
                    // 可以在这里绑定左手控制器的按钮来控制瞄准
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            // 处理拉栓冷却
            if (!canFire && isServer)
            {
                reloadTimer += Time.deltaTime;
                if (reloadTimer >= reloadTime)
                {
                    canFire = true;
                    reloadTimer = 0f;
                }
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

            // 瞄准状态下伤害更高，播放不同震动反馈
            if (hapticEnabled)
            {
                float aimMultiplier = isAiming ? 1.5f : 1.0f;
                PXR_Input.SendHapticImpulse(PXR_Input.VibrateType.RightController,
                    amplitude * aimMultiplier,
                    (int)(duration * aimMultiplier),
                    frequency);
            }

            // 播放射击效果并发送网络命令
            ShowBullet(isServer, isAiming);
            CmdFire(isAiming);

            // 本地进入拉栓状态
            canFire = false;
            reloadTimer = 0f;
        }

        [Command(requiresAuthority = false)]
        public void CmdFire(bool aimed, NetworkConnectionToClient sender = null)
        {
            if (!canFire)
                return;

            // 服务器端逻辑
            if (!isOwned)
            {
                ShowBullet(true, aimed);
            }

            // 广播给其他客户端
            RpcOtherClientsFire(aimed);

            // 服务器设置拉栓冷却
            canFire = false;
            reloadTimer = 0f;
        }

        [ClientRpc]
        public void RpcOtherClientsFire(bool aimed)
        {
            // 其他客户端执行，本地和服务器已执行过，忽略
            if (isOwned || isServer) return;

            ShowBullet(false, aimed);
        }

        // 处理瞄准状态
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

        private void OnAimingChanged(bool oldValue, bool newValue)
        {
            // 调整相机FOV实现瞄准效果
            if (isOwned && playerCamera != null)
            {
                playerCamera.fieldOfView = newValue ? zoomFOV : normalFOV;
            }
        }

        private void ShowBullet(bool isServerBullet, bool isAimed)
        {
            // 播放开火音效
            GameEntry.Sound.PlaySound(weaponCfg.FireSounds, firePoint.position);

            // 播放开火特效
            if (weaponCfg.FlashEffectId > 0)
            {
                GameEntry.Entity.ShowEffect(new EffectData(GameEntry.Entity.GenerateSerialId(), weaponCfg.FlashEffectId)
                {
                    Position = firePoint.position,
                    Rotation = firePoint.rotation
                });
            }

            // 生成子弹实体
            // 瞄准状态下伤害更高
            int damageMultiplier = isAimed ? 2 : 1;

            GameEntry.Entity.ShowBullet(new BulletData(GameEntry.Entity.GenerateSerialId(), weaponCfg.BulletId,
                CampType.Player, NetworkServer.active)
            {
                Position = firePoint.position,
                Rotation = firePoint.rotation,
                Owner = gameObject,
                KeepTime = 10.0f,
                Speed = 50.0f,  // 狙击枪子弹速度更快
                Damage = weaponCfg.Damage * damageMultiplier,
                KnockUp = weaponCfg.KnockUp * damageMultiplier,
                HitEffectId = weaponCfg.HitEffectId,
                HitSoundId = GameEntry.Sound.GetRandomSoundId(weaponCfg.HitSounds)
            });

            // 播放拉栓(如果有)
           
        }
    }
}