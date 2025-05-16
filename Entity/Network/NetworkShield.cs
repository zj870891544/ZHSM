using Mirror;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using ZHSM.cfg;

namespace ZHSM
{
    public class NetworkShield : NetworkWeapon
    {
        [SyncVar(hook = nameof(OnDefendingChanged))][SerializeField]
        protected bool isDefending = false;

        //盾牌碰撞体，在防御状态下启用
        [SerializeField] private Collider shieldCollider;

        //盾牌模型，可以在防御状态下改变位置/角度
        [SerializeField] private Transform shieldModel;

        // 防御姿势的位置偏移和旋转
        [SerializeField] private Vector3 defendingPositionOffset = new Vector3(0.2f, 0, 0.3f);
        [SerializeField] private Vector3 defendingRotationOffset = new Vector3(0, 30, 0);

        private Vector3 originalPosition;
        private Quaternion originalRotation;

       // protected ShieldCfg ShieldConfig => weaponCfg as ShieldCfg;

        [TargetRpc]
        public void RpcSetDefending(bool defending)
        {
            isDefending = defending;
            UpdateDefendingState(defending);
        }

        private void OnDefendingChanged(bool oldValue, bool newValue)
        {
            UpdateDefendingState(newValue);
        }

        //继承自父类的LoadWeaponInfo方法可以处理武器ID
        //这里处理盾牌特有的初始化逻辑
        protected void InitializeShield()
        {
            // 初始化盾牌
            if (shieldModel != null)
            {
                originalPosition = shieldModel.localPosition;
                originalRotation = shieldModel.localRotation;
            }

            // 默认关闭碰撞体
            if (shieldCollider != null)
            {
                shieldCollider.enabled = false;
            }
        }

        private new void Start()
        {
            base.Start();

            if (isOwned)
            {
                isDefending = false;
                // 找到组件
                if (shieldModel == null)
                {
                    shieldModel = transform.Find("Model") as Transform;
                }

                if (shieldCollider == null && shieldModel != null)
                {
                    shieldCollider = shieldModel.GetComponent<Collider>();
                }

                // 保存原始变换
                if (shieldModel != null)
                {
                    originalPosition = shieldModel.localPosition;
                    originalRotation = shieldModel.localRotation;
                }

                // 为新按钮添加监听器（例如副手柄上的按钮）
                // 这里根据实际的XR控制器添加代码
            }
        }

        // 覆盖父类的OnSelectExited方法
        private new void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            // 玩家放下盾牌时的操作
            if (isDefending)
            {
                CmdSetDefending(false);
            }
        }


        /// <summary>
        ///更新防御状态的视觉和功能
        /// </summary>
        /// <param name="defending"></param>
        private void UpdateDefendingState(bool defending)
        {
            if (shieldCollider != null)
            {
                shieldCollider.enabled = defending;
            }

            if (shieldModel != null)
            {
                if (defending)
                {
                    //进入防御姿势
                    shieldModel.localPosition = originalPosition + defendingPositionOffset;
                    shieldModel.localRotation = originalRotation * Quaternion.Euler(defendingRotationOffset);
                }
                else 
                {
                    //恢复原始位置
                    shieldModel.localPosition = originalPosition;
                    shieldModel.localRotation = originalRotation;
                }
            }
        }

        // 覆盖父类的OnTrigger方法，实现盾牌特有的触发行为
        protected override void OnTrigger()
        {
            // 在这里实现盾牌主触发器行为（如果有）
        }

        // 按下副按键时进入防御状态
        public void OnDefendActivate()
        {
            if (!isDefending)
            {
                CmdSetDefending(true);
            }
        }

        // 释放副按键时退出防御状态
        public void OnDefendDeactivate()
        {
            if (isDefending)
            {
                CmdSetDefending(false);
            }
        }

        [Command]
        private void CmdSetDefending(bool defending)
        {
            isDefending = defending;
        }

    }
}

