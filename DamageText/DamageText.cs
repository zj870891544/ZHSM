using TMPro;
using UnityEngine;

namespace ZHSM
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private float keepDuration;
        [SerializeField] private TextMeshPro damageText;
        
        private Transform attachPoint;
        private Transform lookTarget;
        private float keepTimer;

        public void Init(TargetableObject targetable, int damage)
        {
            damageText.text = damage.ToString();
            attachPoint = targetable.DamageTextPoint;

            keepTimer = 0.0f;
        }
        
        private void Start()
        {
            lookTarget = Camera.main.transform;
        }

        private void Update()
        {
            keepTimer += Time.deltaTime;
            if (keepTimer >= keepDuration)
            {
                GameEntry.DamageText.HideDamageText(this);
                return;
            }

            if (attachPoint && lookTarget)
            {
                transform.position = attachPoint.position;
                transform.rotation = Quaternion.LookRotation(lookTarget.position - attachPoint.position);
            }
        }
    }
}