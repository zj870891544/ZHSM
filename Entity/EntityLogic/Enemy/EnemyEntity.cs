using System.Collections.Generic;
using GameFramework.Fsm;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityGameFramework.Runtime;
using ZHSM.cfg;

namespace ZHSM.Enemy
{
    public class EnemyEntity : TargetableObject
    {
#if UNITY_EDITOR
        [Button("受击")]
        public void BeHurt(DamageRequestData damageRequest)
        {
            ApplyDamage(damageRequest);
        }
        
        [Button("强制死亡")]
        public void ForceDead()
        {
            ApplyDamage(new DamageRequestData
            {
                damage = 999999999
            });
        }
#endif
        
        
        
        private static int FSM_SERIAL_ID = 0;
        
        [HideInInspector] public Animator animator;
        [HideInInspector] public NavMeshAgent agent;
        public NetworkTargetable closestTarget;

        private NetworkEnemy networkEnemy;
        private Transform firePoint;
        private EnemyData enemyData;
        private IFsm<EnemyEntity> fsm;

        private EnemyCfg enemyCfg => enemyData.EnemyCfg;
        public EnemyType enemyType => enemyCfg?.Type ?? EnemyType.Unknow;
        public float patrolSpeed => enemyCfg?.PatrolSpeed ?? 0f;
        public float followSpeed => enemyCfg?.FollowSpeed ?? 0f;
        public float followDistance => enemyCfg?.FollowDistance ?? 0f;
        public float attackDistance => enemyCfg?.AttackDistance ?? 0f;
        public float mass => enemyCfg?.Mass ?? 0f;
        public bool PatrolEnable => enemyData.PatrolEnable;
        public float KnockUpSpeed = 15.0f;

        public Vector3 PatrolPosition;
        public void ResetPatrolTarget()
        {
            PatrolPosition = AIUtility.GetInsidePosition(enemyData.PatrolCenter, enemyData.PatrolRadius);
        }
        

        private bool damageRequested = false;
        public bool DamageRequested
        {
            get
            {
                if (damageRequested)
                {
                    damageRequested = false;
                    return true;
                }

                return false;
            }
        }
        public DamageRequestData damageRequestData { get; private set; }

        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawWireSphere(enemyData.PatrolCenter, enemyData.PatrolRadius);
        //     
        //     Gizmos.color = Color.yellow;
        //     Gizmos.DrawWireSphere(transform.position, followDistance);
        //     
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawWireSphere(transform.position, attackDistance);
        // }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            enemyData = userData as EnemyData;
            if (enemyData == null)
            {
                Log.Error("EnemyData is invalid.");
                return;
            }

            // network
            networkEnemy = GetComponent<NetworkEnemy>();
            
            firePoint = transform.Find(enemyCfg.FirePoint);
            
            InvokeRepeating(nameof(RefreshTarget), 1.0f, 1.0f);
            
            // fsm
            fsm = GameEntry.Fsm.CreateFsm((FSM_SERIAL_ID++).ToString(), this,
                new List<FsmState<EnemyEntity>> { new EnemyPatrolState(), new EnemyIdleState(), new EnemyChaseState(), new EnemyAttackState(), new EnemyDamageState(), new EnemyDeadState() });
            fsm.Start<EnemyIdleState>();
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            
            if(fsm != null) GameEntry.Fsm.DestroyFsm(fsm);
        }

        protected override void OnDamage(DamageRequestData damageRequestData)
        {
            base.OnDamage(damageRequestData);

            this.damageRequested = true;
            this.damageRequestData = damageRequestData;

            networkEnemy.RpcTakeDamage(GameEntry.Sound.GetRandomSoundId(enemyCfg.DamageSounds));
        }

        protected override void OnDead(DamageRequestData damageRequestData)
        {
            base.OnDead(damageRequestData);
            
            this.damageRequestData = damageRequestData;

            GameEntry.Event.Fire(this, EnemyDeadEventArgs.Create(enemyData.EnemyId, Entity.Id));
            
            networkEnemy.RpcDead(GameEntry.Sound.GetRandomSoundId(enemyCfg.DeadSounds));
        }
        
        private void RefreshTarget()
        {
            closestTarget = GameEntry.Level.FindClosestTargetable(CachedTransform.position, enemyCfg.TargetCamps);
        }

        public bool CheckDistance(float distance)
        {
            if(!closestTarget) return false;

            return Vector3.Distance(transform.position, closestTarget.Position) <= distance;
        }

        protected virtual void FootStepEvent()
        {
            GameEntry.Sound.PlaySound(GameEntry.Sound.GetRandomSoundId(enemyCfg.FoodStepSounds), transform.position);
        }

        /// <summary>
        /// 近战
        /// </summary>
        protected virtual void MeleeAttack()
        {
            
        }

        /// <summary>
        /// 远程射击
        /// </summary>
        protected virtual void ShootAttack()
        {
            networkEnemy.RpcAttack(firePoint.position, firePoint.rotation, enemyCfg.FlashEffectId,
                GameEntry.Sound.GetRandomSoundId(enemyCfg.FireSounds), enemyCfg.BulletId, enemyCfg.Damage,
                enemyCfg.HitEffectId, GameEntry.Sound.GetRandomSoundId(enemyCfg.HitSounds));
        }
    }
}