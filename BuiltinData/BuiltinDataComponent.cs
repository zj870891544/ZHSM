//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class BuiltinDataComponent : GameFrameworkComponent
    {
        [SerializeField] private TextAsset m_BuildInfoTextAsset = null;
        [SerializeField] private TextAsset m_DefaultDictionaryTextAsset = null;
        [SerializeField] private UpdateResourceForm m_UpdateResourceFormTemplate = null;
        
        [Title("GameModes")]
        [SerializeField] private SurvivalGame m_SurvivalTemplate;
        [SerializeField] private BeachDefenseGame m_BeachDefenseTemplate;
        [SerializeField] private BossFightGame m_BossFightTemplate;
        [Title("Props")]
        [SerializeField] private NetworkPortal m_PortalTemplate;
        
        public UpdateResourceForm UpdateResourceFormTemplate => m_UpdateResourceFormTemplate;
        public SurvivalGame SurvivalTemplate => m_SurvivalTemplate;
        public BeachDefenseGame BeachDefenseTemplate => m_BeachDefenseTemplate;
        public BossFightGame BossFightTemplate => m_BossFightTemplate;
        public NetworkPortal PortalTemplate => m_PortalTemplate;
        
        
        
        private BuildInfo m_BuildInfo = null;

        public BuildInfo BuildInfo
        {
            get
            {
                return m_BuildInfo;
            }
        }


        public void InitBuildInfo()
        {
            if (m_BuildInfoTextAsset == null || string.IsNullOrEmpty(m_BuildInfoTextAsset.text))
            {
                Log.Info("Build info can not be found or empty.");
                return;
            }

            m_BuildInfo = Utility.Json.ToObject<BuildInfo>(m_BuildInfoTextAsset.text);
            if (m_BuildInfo == null)
            {
                Log.Warning("Parse build info failure.");
                return;
            }
        }

        public void InitDefaultDictionary()
        {
            if (m_DefaultDictionaryTextAsset == null || string.IsNullOrEmpty(m_DefaultDictionaryTextAsset.text))
            {
                Log.Info("Default dictionary can not be found or empty.");
                return;
            }

            if (!GameEntry.Localization.ParseData(m_DefaultDictionaryTextAsset.text))
            {
                Log.Warning("Parse default dictionary failure.");
                return;
            }
        }
    }
}
