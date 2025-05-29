using TMPro;
using UnityEngine;

namespace ZHSM
{
    public class WeaponSelector : XRPlaceSelector
    {
        [SerializeField] private TextMeshPro weaponText;
        private void Start()
        {
            weaponText.text = "未选武器";
        }
        
        public override void PlaceItem(XRPlaceItem grabItem)
        {
            base.PlaceItem(grabItem);
            
            WeaponItem weaponItem = grabItem as WeaponItem;
            if (weaponItem)
            {
                GameEntry.WeaponManager.WeaponId = weaponItem.weaponId;
                weaponText.text = weaponItem.weaponId.ToString();
            }
        }

        public override void RemoveItem()
        {
            base.RemoveItem();

            GameEntry.WeaponManager.WeaponId = 0;
            weaponText.text = "未选武器";
        }
    }
}