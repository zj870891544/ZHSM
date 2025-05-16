using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleJSON;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using ZHSM.cfg;

namespace ZHSM.Editor
{
    public class LevelItem
    {
        [HideInInspector]
        public LevelsCfg levelsCfg;
        
        [ReadOnly]
        public string levelName;
        [ReadOnly]
        public SceneAsset sceneAsset;
        
        public Action<LevelsCfg, SceneAsset> OnLoadLevel;

        [Button]
        public void OpenLevel()
        {
            if (!sceneAsset)
            {
                EditorUtility.DisplayDialog("错误", "场景未配置", "确定");
                return;
            }
            
            OnLoadLevel?.Invoke(levelsCfg, sceneAsset);
        }
    }
    
    public class LevelListWindow : OdinEditorWindow
    {
        private TbLevelsCfg tbLevelsCfg;
        private TbSceneCfg tbSceneCfg;
        
        [PropertyOrder(0)]
        [Button("刷新关卡列表", ButtonSizes.Large)]
        public void RefreshLevels()
        {
            levelItems = new List<LevelItem>();
            for (int i = 0; i < tbLevelsCfg.DataList.Count; i++)
            {
                LevelsCfg levelsCfg = tbLevelsCfg.DataList[i];

                string sceneAssetPath = null;
                if (levelsCfg.SceneId > 0)
                {
                    SceneCfg sceneCfg = tbSceneCfg.GetOrDefault(levelsCfg.SceneId);
                    if (sceneCfg != null)
                    {
                        sceneAssetPath = AssetUtility.GetSceneAsset(sceneCfg.AssetName);
                    }
                }
                
                LevelItem levelItem = new LevelItem
                {
                    levelsCfg = levelsCfg,
                    levelName = $"关卡 {levelsCfg.Id}",
                    sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath)
                };
                levelItem.OnLoadLevel += OnLoadLevel;
                
                levelItems.Add(levelItem);
            }
        }

        [Title("关卡列表")]
        [PropertyOrder(1)]
        [TableList(AlwaysExpanded = true, DrawScrollView = true, HideToolbar = true, IsReadOnly = true)]
        public List<LevelItem> levelItems;
        
        protected override async void Initialize()
        {
            base.Initialize();

            Tables tables = new Tables();
            await tables.LoadAsync(Loader);
            
            tbLevelsCfg = tables.TbLevelsCfg;
            tbSceneCfg = tables.TbSceneCfg;

            RefreshLevels();
        }

        private static async Task<JSONNode> Loader(string tableName)
        {
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetUtility.GetDataTableAsset(tableName));
            if (!textAsset)
            {
                Debug.LogError($"load table {tableName} failure.");
                return null;
            }
            return JSON.Parse(textAsset.text);
        }

        private void OnLoadLevel(LevelsCfg levelsCfg, SceneAsset sceneAsset)
        {
            // var levelEditor = GetWindow<LevelEditorWindow3>();
            // levelEditor.Init(levelsCfg, sceneAsset);
        }
    }
}