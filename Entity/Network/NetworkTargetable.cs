using Mirror;
using UnityEngine;

namespace ZHSM
{
    public abstract class NetworkTargetable : NetworkBehaviour
    {
        [SerializeField] [SyncVar] 
        public int SyncedHP;
        [SerializeField] [SyncVar] 
        public CampType SyncedCamp;

        public bool IsDead => SyncedHP <= 0;
        public abstract Vector3 Position { get; }

        public void TakeDamage(int damage)
        {
            SyncedHP -= damage;
        }
        
        // private void OnHPChanged(int oldHP, int newHP)
        // {
        //     ShowHPBar(1.0f * oldHP / MaxHP, 1.0f * newHP / m_TargetableObjectData.MaxHP);
        //     ShowDamageBar(newHP - oldHP);
        // }
        //
        // private void ShowHPBar(float fromHPRatio, float toHPRatio)
        // {
        //     if (SyncedCamp == CampType.Enemy)
        //     {
        //         GameEntry.HPBar.ShowHPBar(this, fromHPRatio, toHPRatio);
        //     }
        // }
        //
        // private void ShowDamageBar(int damage)
        // {
        //     if (!m_DamageTextPoint)
        //     {
        //         Log.Error($"{name} DamageText AttachPoint is null.");
        //         return;
        //     }
        //     
        //     GameEntry.DamageText.ShowDamageText(this, damage);
        // }
    }
}