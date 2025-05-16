using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class PlayerEntity : TargetableObject
    {
        private CharacterData m_CharacterData;
        private NetworkPlayer m_NetworkCharacter;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            m_CharacterData = userData as CharacterData;
            if (m_CharacterData == null)
            {
                Log.Error("PlayerData is invalid in PlayerEntity's OnShow.");
                return;
            }
            
            m_NetworkCharacter = GetComponent<NetworkPlayer>();
        }
    }
}