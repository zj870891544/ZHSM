using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace ZHSM.Editor
{
    public class GameBuildWindow : OdinEditorWindow
    {
        [Button("打包", ButtonSizes.Large)]
        public void BuildApp()
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[] { "Assets/Game Launcher.unity", "Assets/GameLink.unity" };
            buildPlayerOptions.locationPathName = "Build/vr.apk";
            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.options = BuildOptions.None;
        
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
    }
}