using UnityEngine;

namespace ZHSM
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour
    {
        public static BuiltinDataComponent BuiltinData { get; private set; }
        public static LubanTableComponent LubanTable { get; private set; }
        public static MultiPlayerComponent MultiPlayer { get; private set; }
        public static BigSpaceComponent BigSpace { get; private set; }
        public static HPBarComponent HPBar { get; private set; }
        public static DamageTextComponent DamageText { get; private set; }
        public static LevelComponent Level { get; private set; }
        
        public static WeaponManagerComponent WeaponManager { get; private set; }

        private static void InitCustomComponents()
        {
            BuiltinData = UnityGameFramework.Runtime.GameEntry.GetComponent<BuiltinDataComponent>();
            LubanTable = UnityGameFramework.Runtime.GameEntry.GetComponent<LubanTableComponent>();
            MultiPlayer = UnityGameFramework.Runtime.GameEntry.GetComponent<MultiPlayerComponent>();
            BigSpace = UnityGameFramework.Runtime.GameEntry.GetComponent<BigSpaceComponent>();
            HPBar = UnityGameFramework.Runtime.GameEntry.GetComponent<HPBarComponent>();
            DamageText = UnityGameFramework.Runtime.GameEntry.GetComponent<DamageTextComponent>();
            Level = UnityGameFramework.Runtime.GameEntry.GetComponent<LevelComponent>();
            WeaponManager = UnityGameFramework.Runtime.GameEntry.GetComponent<WeaponManagerComponent>();
        }
    }
}
