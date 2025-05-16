#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;

namespace ZHSM.Editor
{
    public class DataTablesWindow : OdinEditorWindow
    {
        public class LubanTableConfig
        {
            public string ConfigSettingPath;
            public string DataTableDir;
            public bool IsDataTableDirValid;
            public string BatFilePath;
            public string CodeOutputPath;
            public string JsonOutputPath;

            public string ProjectCodePath;
            public string ProjectJsonPath;
        }
        
        public static LubanTableConfig LoadLubanConfig()
        {
            LubanTableConfig lubanTableConfig = new LubanTableConfig();
            
            lubanTableConfig.ConfigSettingPath = Path.Combine(Application.persistentDataPath, "ConfigSetting/ConfigSetting.txt");
            
            string savedFolder = File.Exists(lubanTableConfig.ConfigSettingPath) ? File.ReadAllText(lubanTableConfig.ConfigSettingPath).Trim() : null;
            lubanTableConfig.IsDataTableDirValid = !string.IsNullOrEmpty(savedFolder) && Directory.Exists(savedFolder);
            if (lubanTableConfig.IsDataTableDirValid)
            {
                lubanTableConfig.DataTableDir = savedFolder;
                
                lubanTableConfig.BatFilePath = Path.Combine(savedFolder, "gen.bat");
                lubanTableConfig.CodeOutputPath = Path.Combine(savedFolder, "output/code");
                lubanTableConfig.JsonOutputPath = Path.Combine(savedFolder, "output/json");
            }
            
            lubanTableConfig.ProjectCodePath = Path.Combine(Application.dataPath, "GameMain/Scripts/DataTables");
            lubanTableConfig.ProjectJsonPath = Path.Combine(Application.dataPath, "GameMain/DataTables");

            return lubanTableConfig;
        }

        public static void DoGen(string batFile, string codeOutput, string jsonOutput, string projectCode, string projectJson)
        {
            try
            {
                EditorUtility.DisplayProgressBar("Export Datatables", "Processing...", 1.0f);

                //clear output folder
                if (Directory.Exists(projectCode)) FileUtility.ClearFolder(projectCode);
                else Directory.CreateDirectory(projectCode);
                if (Directory.Exists(projectJson)) FileUtility.ClearFolder(projectJson);
                else Directory.CreateDirectory(projectJson);

                //run
                Process process = new Process();
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(batFile);
                process.StartInfo.FileName = batFile;
                process.Start();
                process?.WaitForExit();

                FileUtility.CopyAllFiles(codeOutput, projectCode, ".meta", ".gitkeep");
                FileUtility.CopyAllFiles(jsonOutput, projectJson, ".meta", ".gitkeep");

                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
                EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", e.Message, "OK");
                AssetDatabase.Refresh();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        [Title("数据表目录")] 
        [InfoBox("@folderValidMessage", InfoMessageType.Error, VisibleIf = "@!isFolderInvalid")]
        [OnValueChanged("OnFolderChanged")]
        [FolderPath(AbsolutePath = true)] 
        public string TableFolder;
        private bool isFolderInvalid = false;
        private string folderValidMessage;
        private string configSettingPath;
        
        private string BatFile;
        private string CodeOutput;
        private string JsonOutput;

        [Title("生成目录")] [ReadOnly] public string projectCode;
        [ReadOnly] public string projectJson;

        protected override void Initialize()
        {
            base.Initialize();
            
            LubanTableConfig lubanTableConfig = LoadLubanConfig();

            configSettingPath = lubanTableConfig.ConfigSettingPath;
            isFolderInvalid = lubanTableConfig.IsDataTableDirValid;
            if (isFolderInvalid)
            {
                TableFolder = lubanTableConfig.DataTableDir;

                BatFile = lubanTableConfig.BatFilePath;
                CodeOutput = lubanTableConfig.CodeOutputPath;
                JsonOutput = lubanTableConfig.JsonOutputPath;
            }
            else
            {
                folderValidMessage = "数据目录不存在";
            }

            projectCode = lubanTableConfig.ProjectCodePath;
            projectJson = lubanTableConfig.ProjectJsonPath;
        }

        private void OnFolderChanged()
        {
            isFolderInvalid = Directory.Exists(TableFolder);
            folderValidMessage = isFolderInvalid ? null : "数据表目录不存在";

            if (isFolderInvalid)
            {
                BatFile = Path.Combine(TableFolder, "gen.bat");
                CodeOutput = Path.Combine(TableFolder, "output/code");
                JsonOutput = Path.Combine(TableFolder, "output/json");

                string configSettingFolder = Path.GetDirectoryName(configSettingPath);
                if (!Directory.Exists(configSettingFolder)) Directory.CreateDirectory(configSettingFolder);
                
                File.WriteAllText(configSettingPath, TableFolder, Encoding.UTF8);
            }
        }

        [PropertySpace(20)]
        [EnableIf("isFolderInvalid")]
        [Button("打开数据表目录", ButtonSizes.Large), HorizontalGroup("ButtonsGroup")]
        void OpenDatatableFolder()
        {
            EditorUtility.RevealInFinder(Path.Combine(TableFolder, "datas"));
        }

        [PropertySpace(20)]
        [EnableIf("isFolderInvalid")]
        [GUIColor(0, 1, 0)]
        [Button("导表", ButtonSizes.Large, Icon = SdfIconType.Code), HorizontalGroup("ButtonsGroup")]
        void OnGenData()
        {
            DoGen(BatFile, CodeOutput, JsonOutput, projectCode, projectJson);
        }
    }
}
#endif