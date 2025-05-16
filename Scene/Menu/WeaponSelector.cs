namespace ZHSM
{
    public class WeaponSelector : XRPlaceSelector
    {
        public override void PlaceItem(XRPlaceItem grabItem)
        {
            base.PlaceItem(grabItem);
            
            WeaponItem weaponItem = grabItem as WeaponItem;
            if (weaponItem)
            {
                GameEntry.BigSpace.WeaponId = weaponItem.weaponId;
            }
        }

        public override void RemoveItem()
        {
            base.RemoveItem();

            GameEntry.BigSpace.WeaponId = 0;
        }
    }
}