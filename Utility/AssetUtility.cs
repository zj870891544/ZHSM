//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using Mirror;

namespace ZHSM
{
    public static class AssetUtility
    {
        public static string GetConfigAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/GameMain/Configs/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }
        
        public static string GetDataTableAssetsDir()
        {
            return "Assets/GameMain/DataTables";
        }

        public static string GetDataTableAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/DataTables/{0}.json", assetName);
        }

        public static string GetDictionaryAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/GameMain/Localization/{0}/Dictionaries/{1}.{2}", GameEntry.Localization.Language, assetName, fromBytes ? "bytes" : "xml");
        }

        public static string GetFontAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Fonts/{0}.ttf", assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Scenes/{0}.unity", assetName);
        }

        public static string GetMusicAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Music/{0}.ogg", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Sounds/{0}.ogg", assetName);
        }
        
        public static string GetEntityAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Entities/{0}.prefab", assetName);
        }

        public static string GetUIFormAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UIForms/{0}.prefab", assetName);
        }

        public static string GetUISoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UISounds/{0}.wav", assetName);
        }

        public static string GetWeaponAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Props/Weapons/{0}/prefab/{0}.prefab", assetName);
        }
        
        public static string GetWeaponLocalAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Props/Weapons/{0}/prefab/{0}_Local.prefab", assetName);
        }
        
        public static string GetEnemyAsset(int enemyId)
        {
            return Utility.Text.Format("Assets/GameMain/Enemy/{0}/Prefabs/{0}.prefab", enemyId);
        }
        
        public static string GetBulletAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Bullets/{0}.prefab", assetName);
        }

        public static string GetLevelConfigAsset(int levelId)
        {
            return Utility.Text.Format("Assets/GameMain/Levels/level_{0}.asset", levelId);
        }
    }
}
