using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZHSM
{
    public class PlayerNameplate : MonoBehaviour
    {
        [SerializeField] private Transform canvasRoot;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Vector3 headOffset;

        private Transform cameraTransform;
        private Transform headPoint;

        public void Initialize(GameObject character, bool isLocalPlayer)
        {
            if (isLocalPlayer)
            {
                canvasRoot.gameObject.SetActive(false);
                return;
            }
            
            cameraTransform = Camera.main.transform;
            
            Animator animator = character.GetComponent<Animator>();
            headPoint = animator.GetBoneTransform(HumanBodyBones.Head);
        }

        public void UpdateInfo(string name, float healthRatio)
        {
            nameText.text = name;
            healthSlider.value = healthRatio;

            if (headPoint)
            {
                canvasRoot.position = headPoint.position + headOffset;
                canvasRoot.rotation = Quaternion.identity;
            }
        }
    }
}