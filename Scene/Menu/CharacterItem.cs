using GameFramework.Resource;
using RootMotion.FinalIK;
using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.cfg;

namespace ZHSM
{
    public class CharacterItem : XRPlaceItem
    {
        public int characterId;

        private CharacterCfg characterCfg;
        private Transform m_CharacterRoot;
        protected override Transform targetRoot => m_CharacterRoot;
        
        public string DisplayName => characterCfg?.DisplayName;

        protected override void Initialize()
        {
            base.Initialize();
            
            // load character asset
            TbCharacterCfg tbCharacterCfg = GameEntry.LubanTable.GetTbCharacterCfg();
            characterCfg = tbCharacterCfg.GetOrDefault(characterId);
            
            // characterEntityId = GameEntry.Entity.GenerateSerialId();
            // GameEntry.Entity.ShowCharacter(new CharacterData(characterEntityId, newValue,
            //     CampType.Player, 1));

            // GameEntry.Resource.LoadAsset(characterCfg.AssetPath, new LoadAssetCallbacks(
            //     (string assetname, object asset, float duration, object userdata) =>
            //     {
            //         GameObject playerObj = Instantiate(asset as GameObject, transform);
            //         
            //         m_CharacterRoot = playerObj.transform;
            //         m_CharacterRoot.localPosition = Vector3.zero;
            //         m_CharacterRoot.localRotation = Quaternion.identity;
            //         m_CharacterRoot.localScale = Vector3.one * 0.1f;
            //
            //         VRIK vrIK = playerObj.GetComponent<VRIK>();
            //         vrIK.solver.SetIKPositionWeight(0.0f);
            //         
            //         Collider[] colliders = playerObj.GetComponentsInChildren<Collider>();
            //         foreach (Collider c in colliders)
            //         {
            //             c.enabled = false;
            //         }
            //     },
            //     (string assetname, LoadResourceStatus status, string errormessage, object userdata) =>
            //     {
            //         Log.Error($"load character asset {assetname} failure status:{status}  msg:{errormessage}");
            //     }));
        }
    }
}