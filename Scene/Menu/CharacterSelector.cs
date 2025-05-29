using TMPro;
using UnityEngine;

namespace ZHSM
{
    public class CharacterSelector : XRPlaceSelector
    {
        [SerializeField] private TextMeshPro nameText;

        private void Start()
        {
            nameText.text = "未选择形象";
        }

        public override void PlaceItem(XRPlaceItem grabItem)
        {
            base.PlaceItem(grabItem);
            
            CharacterItem characterItem = grabItem as CharacterItem;
            if (characterItem)
            {
                // GameEntry.BigSpace.CharacterId = characterItem.characterId;
                
                nameText.text = characterItem.DisplayName;
            }
        }

        public override void RemoveItem()
        {
            base.RemoveItem();

            // GameEntry.BigSpace.CharacterId = 0;
            
            nameText.text = "未选择形象";
        }
    }
}