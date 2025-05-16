using DG.Tweening;
using UnityEngine;

namespace ZHSM
{
    public class XRPlaceSelector : MonoBehaviour
    {
        public Transform placePoint;

        private XRPlaceItem placeItem;

        public bool IsItemSelect(XRPlaceItem item)
        {
            return item == placeItem;
        }
        
        public virtual void PlaceItem(XRPlaceItem grabItem)
        {
            if(placeItem) placeItem.ResetOrigin();
            
            placeItem = grabItem;
            placeItem.SetGrabEnable(false);
            placeItem.transform.DOMove(placePoint.position, 0.3f)
                .OnComplete(() => placeItem.SetGrabEnable(true));
            placeItem.transform.DORotateQuaternion(placePoint.rotation, 0.3f);
        }
        
        public virtual void RemoveItem()
        {
            placeItem = null;
        }
    }
}