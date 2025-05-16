//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

namespace ZHSM
{
    /// <summary>
    /// 界面编号。
    /// </summary>
    public enum UIFormId : byte
    {
        Undefined = 0,

        /// <summary>
        /// 弹出框。
        /// </summary>
        DialogForm = 1,
        
        /// <summary>
        /// [服务端] 菜单界面
        /// </summary>
        ServerMenuForm = 100,

        /// <summary>
        /// [服务端]房间等待界面
        /// </summary>
        ServerRoomForm = 101,
        
        /// <summary>
        /// [服务端]游戏房间中界面
        /// </summary>
        ServerGamePlayForm = 102,
    }
}
