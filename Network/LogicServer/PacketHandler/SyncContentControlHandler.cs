using System.Collections.Generic;
using GameFramework.Network;
using Newtonsoft.Json;
using UnityGameFramework.Runtime;

namespace ZHSM.LogicServer
{
    public class SyncContentControlHandler : PacketHandlerBase
    {
        public override int Id => 101;
        
        public override void Handle(object sender, Packet packet)
        {
            SCContentControlData controlData = (SCContentControlData)packet;
            if (controlData == null) return;

            Log.Info($"播控指令 ContentID:{controlData.ContentID} team:{controlData.TeamID}  cmd:{controlData.Cmd} json:{controlData.JsonData}");
            
            switch (controlData.Cmd)
            {
                case 1://暂停
                    GameEntry.Event.Fire(this, ContentControlPauseEventArgs.Create());
                    break;
                case 2://恢复
                    GameEntry.Event.Fire(this, ContentControlResumeEventArgs.Create());
                    break;
                case 3://跳关
                    Dictionary<string, string> seekDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(controlData.JsonData);
                    
                    string stageName = seekDic.GetValueOrDefault("stageName", null);
                    string areaName = seekDic.GetValueOrDefault("areaName", null);
                    if (string.IsNullOrEmpty(stageName) || string.IsNullOrEmpty(areaName))
                    {
                        Log.Error($"播控跳关失败 stageName:{stageName} areaName:{areaName}");
                        return;
                    }
                    
                    GameEntry.Event.Fire(this, ContentControlSeekEventArgs.Create(stageName, areaName));
                    break;
                case 4://退出
                    GameEntry.Event.Fire(this, ContentControlQuitEventArgs.Create());
                    break;
                case 5://开始放映
                    GameEntry.Event.Fire(this, ContentControlStartEventArgs.Create());
                    break;
                case 6://重置身高
                    GameEntry.Event.Fire(this, ContentControlBodyHeightEventArgs.Create());
                    break;
            }
        }
    }
}