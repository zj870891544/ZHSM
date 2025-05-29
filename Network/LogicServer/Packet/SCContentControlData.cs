using System;
using ProtoBuf;

namespace ZHSM.LogicServer
{
    [Serializable, ProtoContract(Name = "SCContentControlData")]
    public class SCContentControlData : SCPacketBase
    {
        public override int Id => 101;
        
        /// <summary>
        /// 播控指令类型:暂停-1，恢复-2，Seek-3，退出-4，开始放映-5，设置身高-6
        /// </summary>
        [ProtoMember(1)]
        public Int32 Cmd;

        /// <summary>
        /// 内容ID
        /// </summary>
        [ProtoMember(2)]
        public string ContentID;

        /// <summary>
        /// 队伍ID
        /// </summary>
        [ProtoMember(3)]
        public string TeamID;

        /// <summary>
        /// 附加数据
        /// </summary>
        [ProtoMember(4)]
        public string JsonData;
        
        public override void Clear()
        {
            Cmd = 0;
            ContentID = null;
            TeamID = null;
            JsonData = null;
        }
    }
}