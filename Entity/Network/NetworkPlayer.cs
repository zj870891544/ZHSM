using System;
using Mirror;
using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.cfg;

namespace ZHSM
{
    public class NetworkPlayer : NetworkTargetable
    {
        [Header("XRRig")]
        public Transform headTransform;
        public Transform lHandTransform;
        public Transform rHandTransform;
        
        private XRRig m_XRRig;
        private int m_WeaponEntityId;
        private Vector3 m_Position;

        public Transform HeadTransform => headTransform;
        public override Vector3 Position
        {
            get
            {
                m_Position = headTransform.position;
                m_Position.y = transform.position.y;
                
                return m_Position;
            }
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            
            m_XRRig = FindObjectOfType<XRRig>();
            
            CmdLoadWeapon(GameEntry.BigSpace.WeaponId);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            gameObject.SetLayerRecursively(isLocalPlayer
                ? LayerMask.NameToLayer("LocalPlayer")
                : LayerMask.NameToLayer("RemotePlayer"));
            
            GameEntry.Event.Fire(this, GamePlayerJoinEventArgs.Create(this));
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            
            GameEntry.Event.Fire(this, GamePlayerLeaveEventArgs.Create(this));
        }
        
        private void Update()
        {
            if (isLocalPlayer)
            {
                headTransform.SetPositionAndRotation(m_XRRig.headTransform.position, m_XRRig.headTransform.rotation);
                lHandTransform.SetPositionAndRotation(m_XRRig.lHandTransform.position, m_XRRig.lHandTransform.rotation);
                rHandTransform.SetPositionAndRotation(m_XRRig.rHandTransform.position, m_XRRig.rHandTransform.rotation);
            }
        }
        
        public void RpcTeleportTo(Vector3 position, Quaternion rotation)
        {
            if (!isLocalPlayer) return;
            
            Debug.Log($"[Player-{netId}] 跳转至出生点 >>> ");
            
            XRRig.instance.TeleportTo(position, rotation);
        }
        
        [Command]
        private void CmdLoadWeapon(int weaponId)
        {
            Debug.Log("服务端加载武器 >>> " + weaponId);
            
            TbWeaponCfg tbWeaponCfg = GameEntry.LubanTable.GetTbWeaponCfg();
            WeaponCfg weaponCfg = tbWeaponCfg.GetOrDefault(weaponId);
            if (weaponCfg == null)
            {
                Log.Error($"Weapon {weaponId} not found.");
                return;
            }

            m_WeaponEntityId = GameEntry.Entity.GenerateSerialId();
            GameEntry.Entity.ShowWeapon(new WeaponData(m_WeaponEntityId, weaponCfg.EntityId, weaponId, connectionToClient)
            {
                Position = transform.position
            });
        }
    }
}