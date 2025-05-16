using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.cfg;

namespace ZHSM
{
    [System.Serializable]
    public class CharacterData : TargetableObjectData
    {
        [SerializeField] private uint m_NetId;
        [SerializeField] private int m_CharacterId;
        [SerializeField] private int m_MaxHP = 0;
        
        public CharacterData(int entityId, int typeId, CampType camp, uint netId, int characterId) : base(entityId, typeId, camp)
        {
            m_NetId = netId;
            m_CharacterId = characterId;

            TbCharacterCfg tbCharacterCfg = GameEntry.LubanTable.GetTbCharacterCfg();
            CharacterCfg characterCfg = tbCharacterCfg.GetOrDefault(m_CharacterId);
            if (characterCfg == null)
            {
                Log.Error($"Player {m_CharacterId} not found.");
                return;
            }

            m_MaxHP = HP = characterCfg.HP;
        }

        public uint NetId => m_NetId;
        public int CharacterId => m_CharacterId;
        public override int MaxHP => m_MaxHP;
    }
}