using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleJSON;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZHSM.cfg;

namespace ZHSM.Editor
{
    public class LevelEditorWindow : OdinMenuEditorWindow
    {
        private TbSceneCfg tbSceneCfg;
        private TbLevelsCfg tbLevelsCfg;
        private List<LevelsCfg> levelsCfgs;
        
        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true);
            tree.Selection.SupportsMultiSelect = false;

            return tree;
        }

        protected override async void Initialize()
        {
            base.Initialize();
            
            Tables tables = new Tables();
            await tables.LoadAsync(Loader);

            tbSceneCfg = tables.TbSceneCfg;
            levelsCfgs = tables.TbLevelsCfg.DataList;
            
            for (int i = 0; i < levelsCfgs.Count; i++)
            {
                LevelsCfg levelsCfg = levelsCfgs[i];

                LevelConfig levelConfig =
                    AssetDatabase.LoadAssetAtPath<LevelConfig>(AssetUtility.GetLevelConfigAsset(levelsCfg.Id));
                MenuTree.AddObjectAtPath($"level_{levelsCfg.Id}", levelConfig);
            }
            
            MenuTree.Selection.SelectionChanged += SelectionOnSelectionChanged;
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

        private void SelectionOnSelectionChanged(SelectionChangedType selectionChangedType)
        {
            if (selectionChangedType != SelectionChangedType.ItemAdded) return;
            
            int menuIndex = 0;
            for (int i = 0; i < MenuTree.MenuItems.Count; i++)
            {
                int idx = MenuTree.Selection.IndexOf(MenuTree.MenuItems[i]);
                if (idx == 0)
                {
                    menuIndex = i;
                    break;
                }
            }
            
            LevelsCfg levelsCfg = levelsCfgs[menuIndex];
            
            SceneCfg sceneCfg = tbSceneCfg.GetOrDefault(levelsCfg.SceneId);
            if (sceneCfg == null)
            {
                EditorUtility.DisplayDialog("错误", "场景未配置", "确定");
                return;
            }
            
            string sceneAssetPath = AssetUtility.GetSceneAsset(sceneCfg.AssetName);
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath);
            if (!sceneAsset)
            {
                EditorUtility.DisplayDialog("错误", "场景资源不存在", "确定");
                return;
            }
            
            LevelConfig levelConfig = MenuTree.Selection.SelectedValue as LevelConfig;
            if (!levelConfig)
            {
                if (EditorUtility.DisplayDialog("配置缺失", "关卡配置文件不存在，是否创建？", "创建", "取消"))
                {
                    
                    
                    MenuTree.MenuItems[menuIndex].Value = levelConfig;
                }
            }

            LoadLevel(levelConfig, sceneAssetPath);
        }

        private LevelConfig levelConfig;
        private Transform portalGroup;
        
        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
        
        void OnSceneGUI(SceneView sceneView)
        {
            levelConfig?.DrawGizmos();
            
            sceneView.Repaint();
        }
        
        public void LoadLevel(LevelConfig _levelConfig, string sceneAssetPath)
        {
            levelConfig = _levelConfig;
            
            //open scene
            EditorSceneManager.OpenScene(sceneAssetPath, OpenSceneMode.Single);
        }
    }
}