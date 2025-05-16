//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

namespace ZHSM
{
    /// <summary>
    /// 阵营类型。
    /// </summary>
    public enum CampType : byte
    {
        Unknown = 0,

        /// <summary>
        /// 玩家阵营
        /// </summary>
        Player = 1,

        /// <summary>
        /// 敌方阵营
        /// </summary>
        Enemy = 2,
        
        /// <summary>
        /// 玩家防御塔
        /// </summary>
        PlayerDefenseTower = 3
    }
}
