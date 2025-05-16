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
            GUILayout.BeginArea(_rect);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("角色：");
            characterId = GUILayout.TextField(characterId);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("武器：");
            weaponId = GUILayout.TextField(weaponId);
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button("Start Game"))
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