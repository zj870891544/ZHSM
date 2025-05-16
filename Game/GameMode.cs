namespace ZHSM
{
    /// <summary>
    /// 游戏模式
    /// </summary>
    public enum GameMode : byte
    {
        /// <summary>
        /// 常规生存割草
        /// </summary>
        Survival = 1,
        
        /// <summary>
        /// 抢滩登陆玩法
        /// </summary>
        BeachDefense = 2,
        
        /// <summary>
        /// Boss战
        /// </summary>
        BossFight = 3
    }
}