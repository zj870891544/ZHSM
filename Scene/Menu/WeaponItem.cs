using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.cfg;

namespace ZHSM
{
    public class WeaponItem : XRPlaceItem
    {
        public int weaponId;

        private Transform m_WeaponRoot;
        protected override Transform targetRoot => m_WeaponRoot;

        protected override void Initialize()
        {
            base.Initialize();
            
            TbWeaponCfg tbWeaponCfg = GameEntry.LubanTable.GetTbWeaponCfg();
            WeaponCfg weaponCfg = tbWeaponCfg.GetOrDefault(weaponId);
            
            // string weaponAssetPath = AssetUtility.GetWeaponLocalAsset(weaponCfg.AssetName);
            // GameEntry.Resource.LoadAsset(weaponAssetPath, new LoadAssetCallbacks(
            //     (string assetname, object asset, float duration, object userdata) =>
            //     {
            //         GameObject weaponObj = Instantiate(asset as GameObject, transform);
            //
            //         m_WeaponRoot = weaponObj.transform;
            //         m_WeaponRoot.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            //         m_WeaponRoot.localScale = Vector3.one * 0.4f;
            //     },
            //     (string assetname, LoadResourceStatus status, string errormessage, object userdata) =>
            //     {
            //         Log.Error($"load character asset {assetname} failure status:{status}  msg:{errormessage}");
            //     }));
        }
    }
}