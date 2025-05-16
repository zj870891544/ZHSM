using System.Diagnostics;
using System.IO;
using System.Linq;
using BennyKok.ToolbarButtons;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ZHSM.Editor
{
    public class CustomToolbarButtons
    {
        private const string scenesFolder = "Scenes";
        private static AdvancedDropdownState scenesState = new AdvancedDropdownState();
        
        [ToolbarButton("launch", "启动游戏", order = -2)]
        public static void LaunchGame()
        {
            Debug.Log("启动游戏...");
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/Game Launcher.unity", OpenSceneMode.Single);
                EditorApplication.EnterPlaymode();
            }
        }
        
        [ToolbarButton("scenes", "选择场景", order = -1)]
        public static void ShowScenes()
        {
            var sceneList = AssetDatabase.GetAllAssetPaths().Where(s => s.EndsWith(".unity")).ToList();
            sceneList.Sort();
        
            const string prefKey = "ToolbarScenesState";
            var jsonState = EditorPrefs.GetString(prefKey);
            if (!string.IsNullOrEmpty(jsonState))
            {
                EditorJsonUtility.FromJsonOverwrite(jsonState, scenesState);
            }
            var a = new GenericAdvancedDropdown("All Scenes", scenesState);
            foreach (var p in sceneList)
            {
                string label = ReplaceLast(p, ".unity", "");
                label = ReplaceFirst(label, "Assets/", "");
                a.AddItem(label, () =>
                {
                    EditorPrefs.SetString(prefKey, EditorJsonUtility.ToJson(scenesState));
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(p, OpenSceneMode.Single);
                    }
                });
            }
            a.AddItem("New Scene +", () =>
            {
                EditorApplication.ExecuteMenuItem("File/New Scene");
            });
            a.ShowAsContext(10);
        }
        
        [ToolbarButton("level", "关卡", order = 1)]
        public static void OpenLevelEditor()
        {
            LevelEditorWindow levelEditor = EditorWindow.GetWindow<LevelEditorWindow>();
            levelEditor.Show();
        }
        
        [ToolbarButton("datatable", "数据表", order = 2)]
        public static void ImportDatatable()
        {
            var a = new GenericMenu();
            a.AddItem(new GUIContent("导表"), false, () =>
            {
                DataTablesWindow.LubanTableConfig tableConfig = DataTablesWindow.LoadLubanConfig();

                if (tableConfig.IsDataTableDirValid)
                    DataTablesWindow.DoGen(tableConfig.BatFilePath, tableConfig.CodeOutputPath,
                        tableConfig.JsonOutputPath, tableConfig.ProjectCodePath, tableConfig.ProjectJsonPath);
                else
                    EditorUtility.DisplayDialog("错误", "数据表目录未配置", "确定");
            });
            a.AddItem(new GUIContent("导表配置"), false, () =>
            {
                DataTablesWindow win = EditorWindow.GetWindow<DataTablesWindow>();
                win.Show();
            });
            a.ShowAsContext();
        }

        [ToolbarButton("build", "打包构建", order = 11)]
        public static void BuildPlayer()
        {
            EditorWindow.GetWindow<GameBuildWindow>().Show();
        }

        [ToolbarButton("d_winbtn_win_max", "启动热更资源服务器", order = 12)]
        public static void OpenTerminal()
        {
            string rootPath = Directory.GetCurrentDirectory();
            string batPath = Path.Combine(rootPath, "Plugins/server.bat");
            
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = batPath,
                WorkingDirectory = Path.Combine(rootPath, "Bundles/WWW/Debug")
            };

            Process.Start(startInfo);
        }

        [ToolbarButton("Settings", "Show Settings", order = 1000)]
        public static void ShowSettings()
        {
            var a = new GenericMenu();
            a.AddItem(new GUIContent("Project"), false, () => EditorApplication.ExecuteMenuItem("Edit/Project Settings..."));
            a.AddItem(new GUIContent("Preferences"), false, () => EditorApplication.ExecuteMenuItem("Edit/Preferences..."));
            a.ShowAsContext();
        }
        
        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        
        public static string ReplaceLast(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);
        
            if (place == -1)
                return Source;
        
            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }
    }
}