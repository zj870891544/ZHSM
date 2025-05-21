using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

namespace ZHSM
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private XRPushButton m_StartButton;

        [SerializeField] private Rect _rect;
        private string characterId = "10001";
        private string weaponId = "10003";

        private void OnGUI()
        {
            // 创建统一的大字体样式
            GUIStyle bigFontStyle = new GUIStyle(GUI.skin.label);
            bigFontStyle.fontSize = 18; // 增大字体大小

            // 为文本框创建专用样式
            GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);
            textFieldStyle.fontSize = 18; // 文本框字体大小
            textFieldStyle.padding = new RectOffset(10, 10, 10, 10); // 增加内边距

            // 为按钮创建专用样式
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 18; // 按钮字体大小
            buttonStyle.padding = new RectOffset(20, 20, 15, 15); // 按钮内边距

            GUILayout.BeginArea(_rect);

            // 角色行
            GUILayout.BeginHorizontal();
            GUILayout.Label("角色：", bigFontStyle, GUILayout.Height(40)); // 应用大字体样式
            characterId = GUILayout.TextField(characterId, textFieldStyle, GUILayout.Height(40));
            GUILayout.EndHorizontal();

            // 武器行
            GUILayout.BeginHorizontal();
            GUILayout.Label("武器：", bigFontStyle, GUILayout.Height(40)); // 应用大字体样式
            weaponId = GUILayout.TextField(weaponId, textFieldStyle, GUILayout.Height(40));
            GUILayout.EndHorizontal();

            // 按钮
            if (GUILayout.Button("Start Game", buttonStyle, GUILayout.Height(60)))
            {
                GameEntry.BigSpace.CharacterId = int.Parse(characterId);
                GameEntry.BigSpace.WeaponId = int.Parse(weaponId);
                StartGame();
            }

            GUILayout.EndArea();
        }

        private void Start()
        {
            m_StartButton.enabled = true;
        }

        [Button(ButtonSizes.Large)]
        public void StartGame(int characterId = 10001, int weaponId = 10003)
        {
            GameEntry.BigSpace.CharacterId = characterId;
            GameEntry.BigSpace.WeaponId = weaponId;
            
            StartGame();
        }

        
        public void StartGame()
        {
            if (GameEntry.BigSpace.CharacterId <= 0 || GameEntry.BigSpace.WeaponId <= 0) return;
            
            m_StartButton.enabled = false;
            
            ProcedureMenu procedureMenu = GameEntry.Procedure.CurrentProcedure as ProcedureMenu;
            procedureMenu?.StartGame();
        }
    }
}