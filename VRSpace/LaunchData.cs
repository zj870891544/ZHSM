using System;
using Sirenix.OdinInspector;

namespace ZHSM
{
    [System.Serializable]
    public class LaunchData
    {
        [LabelText("场馆ID")]
        public string venueID;
        [LabelText("房间ID")]
        public string roomID;
        [LabelText("房间名称")]
        public string roomName;
        
        [LabelText("内容ID")]
        public string contentID;
        public string contentName;
        public string contentVersion;

        [PropertySpace]
        public string secretKey;
        public string checkFrequency;
        public Int64 startTime;
        
        [PropertySpace]
        public string teamOriginID;
        public string teamName;
        public string teamCount;
        public string playerOriginID;
        public string playerNick;
        public string languageType;

        public LaunchExtraData jsonData;
    }

    [System.Serializable]
    public class LaunchExtraData
    {
        public string serverIP;
        public int serverPort;
        public string bridgeServerIP;
        public int bridgeServerPort;
        public string fileUpdateURL;
        public string telephone;
        public string iconLink;
        
        public string maxServerIP;
        public int maxServerPort;
        public string maxConfigUpdateURL;

        public string avatarResourceName;
        public string avatarSkinResourceName;
        
        public string contentOffsetUrl;
        public string versionID;
        public string deviceId;
        public string exJson;
    }
}