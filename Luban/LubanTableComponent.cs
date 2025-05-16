using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using System.Threading.Tasks;
using GameFramework.Resource;
using SimpleJSON;
using ZHSM.cfg;

namespace ZHSM
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Luban DataTable")]
    public sealed class LubanTableComponent : GameFrameworkComponent
    {
        private ILubanTables m_LubanTables = null;

        private Dictionary<string, bool> m_LoadedFlag = new Dictionary<string, bool>();
        private Dictionary<string, TaskCompletionSource<TextAsset>> m_LubanTableTcs = new Dictionary<string, TaskCompletionSource<TextAsset>>();

        public bool TryGetTables<T>(out T tables) where T : class
        {
            tables = null;
            if (m_LubanTables != null && m_LubanTables is T)
            {
                tables = m_LubanTables as T;
                return true;
            }
            
            Log.Error("Please initialize luban tables first!");
            return false;
        }
        
        public T GetTables<T>() where T : class
        {
            if (m_LubanTables != null && m_LubanTables is T)
            {
                return m_LubanTables as T;
            }
            
            Log.Error("Please initialize luban tables first!");
            return default;
        }

        public async void LoadLubanTable<T>(string loadFlagKey, object userData) where T : ILubanTables, new()
        {
            m_LoadedFlag.Clear();
            m_LubanTableTcs.Clear();

            var tables = new T();
            await tables.LoadAsync( file => LoadLubanJsonAsync(file));
            if (IsLoadSuccssful())
            {
                m_LubanTables = tables;
                GameEntry.Event.Fire(this,LoadLubanTableSuccessEventArgs.Create(loadFlagKey,userData));
            }
            else
            {
                GameEntry.Event.Fire(this,LoadLubanTableSuccessEventArgs.Create(loadFlagKey,userData));
            }
        }

        private bool IsLoadSuccssful()
        {
            foreach (var flag in m_LoadedFlag)
            {
                if (!flag.Value)
                    return false;
            }

            return true;
        }

        private async Task<JSONNode> LoadLubanJsonAsync(string file)
        {
            return JSON.Parse((await LoadLubanTableAssetAsync(file)).text);
        }
        
        private Task<TextAsset> LoadLubanTableAssetAsync(string assetName)
        {
            var tcs = new TaskCompletionSource<TextAsset>();
            string assetFullName = AssetUtility.GetDataTableAsset(assetName);
            m_LubanTableTcs.Add(assetFullName,tcs);
            m_LoadedFlag.Add(assetFullName, false);

            GameEntry.Resource.LoadAsset(assetFullName,new LoadAssetCallbacks((string assetName, object asset, float duration, object userData) =>
            {
                if (m_LubanTableTcs.TryGetValue(assetName, out TaskCompletionSource<TextAsset> loadTcs))
                {
                    m_LoadedFlag[assetName] = true;
                    loadTcs.SetResult(asset as TextAsset);
                    m_LubanTableTcs.Remove(assetName);
                }
            }, (string assetName, LoadResourceStatus status, string errorMessage,
                object userData) =>
            {
                m_LubanTableTcs.TryGetValue(assetName, out TaskCompletionSource<TextAsset> tcs);
                if (tcs != null)
                {
                    tcs.SetCanceled();
                    m_LubanTableTcs.Remove(assetName);
                }
                
                Log.Error("Can not load luban table from '{0}' with error message '{1}'.", assetName, errorMessage);
            }));
            
            return tcs.Task;
        }
        
        public TbSceneCfg GetTbSceneCfg()
        {
            if (GameEntry.LubanTable.TryGetTables(out cfg.Tables tables))
            {
                return tables.TbSceneCfg;
            }
        
            return null;
        }

        public TbEntityCfg GetTbEntityCfg()
        {
            if (GameEntry.LubanTable.TryGetTables(out cfg.Tables tables))
            {
                return tables.TbEntityCfg;
            }

            return null;
        }

        public TbUIFormCfg GetTbUIFormCfg()
        {
            if (GameEntry.LubanTable.TryGetTables(out cfg.Tables tables))
            {
                return tables.TbUIFormCfg;
            }

            return null;
        }
        
        public TbCharacterCfg GetTbCharacterCfg()
        {
            if (GameEntry.LubanTable.TryGetTables(out cfg.Tables tables))
            {
                return tables.TbCharacterCfg;
            }
        
            return null;
        }
        
        public TbEnemyCfg GetTbEnemyCfg()
        {
            if (GameEntry.LubanTable.TryGetTables(out cfg.Tables tables))
            {
                return tables.TbEnemyCfg;
            }
        
            return null;
        }
        
        public TbWeaponCfg GetTbWeaponCfg()
        {
            if (GameEntry.LubanTable.TryGetTables(out cfg.Tables tables))
            {
                return tables.TbWeaponCfg;
            }
        
            return null;
        }
        
        public TbLevelsCfg GetTbLevelsCfg()
        {
            if (GameEntry.LubanTable.TryGetTables(out cfg.Tables tables))
            {
                return tables.TbLevelsCfg;
            }
        
            return null;
        }
    }
}


