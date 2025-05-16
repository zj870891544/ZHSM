using System.Collections.Generic;
using GameFramework.Event;
using GameFramework.Resource;
using Mirror;
using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.cfg;
using ZHSM.Enemy;

namespace ZHSM
{
    public abstract class GameBase : NetworkBehaviour
    {
        protected int levelId;
        protected int waveIndex = 0;
        protected TbEnemyCfg tbEnemyCfg;
        protected LevelsCfg levelsCfg;
        protected LevelConfig levelConfig;
        
        private Dictionary<string, bool> levelEvents;
        
        public abstract GameMode GameMode
        {
            get;
        }

        public bool GameOver
        {
            get;
            protected set;
        }

        public virtual void Initialize(object gameData = null)
        {
            GameEntry.Event.Subscribe(EnemyDeadEventArgs.EventId, OnEnemyDead);
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
            GameEntry.Event.Subscribe(LevelEventTriggerEventArgs.EventId, OnLevelEventTrigger);
            
            if(NetworkServer.active) 
                NetworkServer.Spawn(gameObject);

            if (gameData == null)
            {
                OnInitializeFailure($"gameData is null in GameBase's Initialize   {GameMode}");
                return;
            }
            
            levelId = (int)gameData;
            GameOver = false;
            waveIndex = 0;
            levelEvents = new Dictionary<string, bool>();
            
            tbEnemyCfg = GameEntry.LubanTable.GetTbEnemyCfg();

            TbLevelsCfg tbLevelsCfg = GameEntry.LubanTable.GetTbLevelsCfg();
            levelsCfg = tbLevelsCfg.GetOrDefault(levelId);
            if (levelsCfg == null)
            {
                OnInitializeFailure("levelsCfg is null in SurvivalGame.Initialize");
                return;
            }
            
            // 加载关卡配置
            string levelConfigAsset = AssetUtility.GetLevelConfigAsset(levelId);
            GameEntry.Resource.LoadAsset(levelConfigAsset, new LoadAssetCallbacks((assetName, asset, duration, data) =>
            {
                levelConfig = asset as LevelConfig;
                if (levelConfig != null)
                {
                    OnInitializeSuccess(levelConfig);
                }
                else
                {
                    OnInitializeFailure($"Load LevelConfig parse error: {assetName}");
                }
            }, (assetName, status, message, data) =>
            {
                Log.Error($"Load LevelConfig {assetName} status:{status} error:{message}");
            }));
        }
        
        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }
        
        public virtual void Shutdown()
        {
            GameEntry.Event.Unsubscribe(EnemyDeadEventArgs.EventId, OnEnemyDead);
            GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
            GameEntry.Event.Unsubscribe(LevelEventTriggerEventArgs.EventId, OnLevelEventTrigger);
            
            if(NetworkServer.active) 
                NetworkServer.UnSpawn(gameObject);
        }

        protected virtual void OnInitializeSuccess(LevelConfig levelConfig)
        {
            Log.Info($"[{levelId}]{GameMode} 初始化成功 >>> ");
            
            
            // TODO: 需要等待所有玩家加入
        }

        protected virtual void OnInitializeFailure(string error)
        {
            Log.Error($"[{levelId}]{GameMode} 初始化失败 >>> {error}");
        }
        
        protected virtual void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            
        }

        protected virtual void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            Log.Warning("Show entity failure with error message '{0}'.", ne.ErrorMessage);
        }
        
        protected virtual void OnEnemyDead(object sender, GameEventArgs e)
        {
            
        }

        protected void AddLevelEvent(string eventName)
        {
            levelEvents.TryAdd(eventName, false);
        }

        protected bool IsEventTriggered(string eventName)
        {
            return levelEvents.GetValueOrDefault(eventName, false);
        }

        protected void ClearLevelEvents()
        {
            levelEvents.Clear();
        }
        
        private void OnLevelEventTrigger(object sender, GameEventArgs e)
        {
            LevelEventTriggerEventArgs ne = e as LevelEventTriggerEventArgs;
            if (ne == null || !levelEvents.ContainsKey(ne.EventName)) return;

            Log.Info($"{levelId}-{waveIndex} 事件完成 >>> {ne.EventName}");
            levelEvents[ne.EventName] = true;
        }
        
        
        protected void ShowPortals()
        {
            if (levelsCfg.IsFinalLevel)
            {
                Log.Info("游戏结束 >>> ");
                return;
            }
            
            Log.Info($"{levelsCfg.Id} 创建传送门 >>> ");
            if (levelConfig.portals == null)
            {
                Log.Error("传送门数据配置错误");
                return;
            }

            foreach (LevelPortalConfig portalConfig in levelConfig.portals)
            {
                NetworkPortal portalObj = Instantiate(GameEntry.BuiltinData.PortalTemplate, portalConfig.position, portalConfig.rotation);
                portalObj.Initialize(portalConfig);
                
                NetworkServer.Spawn(portalObj.gameObject);
            }
        }


        #region RPC
        [ClientRpc]
        protected void RpcLevelStart(int levelId)
        {
            Debug.Log($"[RPC] {levelId} 关卡开始 >>> ");
            GameEntry.Event.Fire(this, LevelStartEventArgs.Create(levelId));
        }

        [ClientRpc]
        protected void RpcLevelComplete(int levelId)
        {
            Debug.Log($"[RPC] {levelId} 关卡结束 >>> ");
            GameEntry.Event.Fire(this, LevelCompleteEventArgs.Create(levelId));
        }

        [ClientRpc]
        protected void RpcLevelEventActivate(string eventName)
        {
            Debug.Log($"[RPC] 激活事件 >>> {eventName} ");
            GameEntry.Event.Fire(this, LevelEventActivateEventArgs.Create(eventName));
        }
        #endregion
    }
}