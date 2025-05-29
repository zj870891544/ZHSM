using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZHSM
{
    /// <summary>
    /// 命令行参数获取 & 解析工具类
    /// </summary>
    public static class LauncherParamHelper
    {
        public const string VenueId = "venueID";
        public const string RoomId = "roomID";
        public const string RoomName = "roomName";
        public const string ContentIdIndex = "contentID";
        public const string ContentNameIndex = "contentName";
        public const string ContentVersionIndex = "contentVersion";
        public const string SecretKeyIndex = "secretKey";
        public const string StartTimeIndex = "startTime";
        public const string TeamIdIndex = "teamOriginID";
        public const string TeamNameIndex = "teamName";
        public const string TeamCount = "teamCount";
        public const string playerIdIndex = "playerOriginID";
        public const string NickIndex = "playerNick";
        public const string JsonData = "jsonData";

        /// <summary>
        /// 通过启动传参获取用户nick
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetLauncherParams(string[] keys)
        {
            if (keys == null)
            {
                return new Dictionary<string, string>();
            }

            Dictionary<string, string> results = new Dictionary<string, string>(keys.Length);

#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (unityPlayer == null)
            {
                return results;
            }

            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            if (currentActivity == null)
            {
                return results;
            }


            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            if (intent != null)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    if (intent.Call<bool>("hasExtra", keys[i]))
                    {
                        string extraData = intent.Call<string>("getStringExtra", keys[i]);

                        results.Add(keys[i], extraData);
                    }
                }
            }

#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
            string[] param = Environment.GetCommandLineArgs();
            if (param != null && param.Length > 1)
            {
                for (int i = 1; i < param.Length - 1; i += 2)
                {
                    results.Add(param[i], param[i + 1]);
                }
            }

#endif
            return results;
        }
    }
}