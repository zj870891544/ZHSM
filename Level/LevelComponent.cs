using System.Collections.Generic;
using GameFramework.Event;
using Mirror;
using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.cfg;

namespace ZHSM
{
    public class LevelComponent : GameFrameworkComponent
    {
        #region Level
        public int StartLevelId = 1;
        
        private LevelsCfg m_CurrentLevelsCfg;
        public int CurrentLevel { get; private set; }
        
        public void LoadLevel(int levelId)
        {
            Log.Info($"加载关卡: {levelId}");
            
            CurrentLevel = levelId;
            
            TbLevelsCfg tbLevelsCfg = GameEntry.LubanTable.GetTbLevelsCfg();
            LevelsCfg levelsCfg = tbLevelsCfg.GetOrDefault(levelId);
            if (levelsCfg == null)
            {
                Log.Error($"load level {levelId} failure: not exist.");
                return;
            }
            m_CurrentLevelsCfg = levelsCfg;

            TbSceneCfg tbSceneCfg = GameEntry.LubanTable.GetTbSceneCfg();
            SceneCfg sceneCfg = tbSceneCfg.GetOrDefault(m_CurrentLevelsCfg.SceneId);
            if (sceneCfg == null)
            {
                Log.Error($"load scene {m_CurrentLevelsCfg.SceneId} failure: not exist.");
                return;
            }
            
            string sceneAssetPath = AssetUtility.GetSceneAsset(sceneCfg.AssetName);
            GameEntry.MultiPlayer.LoadScene(sceneAssetPath);
        }
        #endregion
        
        #region Targetables
        [SerializeField]
        private Dictionary<CampType, List<NetworkTargetable>> m_Targetables = new Dictionary<CampType, List<NetworkTargetable>>();
        public List<NetworkTargetable> Players => m_Targetables?[CampType.Player];

        public void AddTargetable(CampType campType, NetworkTargetable networkTargetable)
        {
            if (!m_Targetables.ContainsKey(campType))
                m_Targetables.Add(campType, new List<NetworkTargetable>());

            if (m_Targetables.TryGetValue(campType, out List<NetworkTargetable> targetables))
            {
                if(!targetables.Contains(networkTargetable))
                    targetables.Add(networkTargetable);
            }
        }

        public void RemoveTargetable(CampType campType, NetworkTargetable networkTargetable)
        {
            if (m_Targetables.TryGetValue(campType, out List<NetworkTargetable> targetables))
            {
                if (targetables.Contains(networkTargetable))
                    targetables.Remove(networkTargetable);
            }
        }

        public void ClearTargetables()
        {
            m_Targetables.Clear();
        }
        
        public bool IsPlayerExist(uint playerNetId)
        {
            if (m_Targetables.TryGetValue(CampType.Player, out List<NetworkTargetable> targetables))
            {
                foreach (NetworkTargetable targetable in targetables)
                {
                    if (targetable.netId == playerNetId) return true;
                }
            }

            return false;
        }

        public bool IsAllPlayersExist(HashSet<uint> playerNetIds)
        {
            if(playerNetIds == null || playerNetIds.Count <= 0) return false;
            
            if (m_Targetables.TryGetValue(CampType.Player, out List<NetworkTargetable> targetables))
            {
                foreach (NetworkTargetable targetable in targetables)
                {
                    if (!playerNetIds.Contains(targetable.netId)) return false;
                }

                return true;
            }

            return false;
        }

        public NetworkTargetable FindClosestTargetable(Vector3 startPos, List<int> camps)
        {
            if (camps is not { Count: > 0 })
            {
                Log.Error("FindClosesTargetable error: camps is null or empty.");
                return null;
            }
            
            float closestDist = float.MaxValue;
            NetworkTargetable resultTarget = null;

            for (int i = 0; i < camps.Count; i++)
            {
                CampType camp = (CampType)camps[i];
                if (m_Targetables.TryGetValue(camp, out List<NetworkTargetable> targetables))
                {
                    foreach (NetworkTargetable targetable in targetables)
                    {
                        if(targetable.IsDead) continue;
                    
                        float dist = Vector3.Distance(targetable.Position, startPos);
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            resultTarget = targetable;
                        }
                    }
                    
                    if(resultTarget) break;
                    
                    closestDist = float.MaxValue;
                }
            }

            return resultTarget;
        }
        
        // /// <summary>
        // /// 查找距离最近的玩家
        // /// </summary>
        // /// <param name="centerPosition"></param>
        // /// <returns></returns>
        // public NetworkPlayer FindClosestPlayer(Vector3 centerPosition)
        // {
        //     if (GameEntry.Level.PlayersCount <= 0) return null;
        //     
        //     float closestDist = float.MaxValue;
        //     NetworkPlayer resultPlayer = null;
        //
        //     foreach (KeyValuePair<uint, NetworkPlayer> kv in playerDictionary)
        //     {
        //         if (kv.Value.IsDead) continue;
        //         
        //         float dist = Vector3.Distance(kv.Value.Position, centerPosition);
        //         if (dist < closestDist)
        //         {
        //             closestDist = dist;
        //             resultPlayer = kv.Value;
        //         }
        //     }
        //
        //     return resultPlayer;
        // }
        #endregion

        private Dictionary<int, NetworkConnectionToClient> playerConnections = new Dictionary<int, NetworkConnectionToClient>();
        private void Start()
        {
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ServerAddPlayerEventArgs.EventId, OnServerAddPlayer);
        }

        private void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = e as ShowEntitySuccessEventArgs;
            if(ne == null) return;

            if (playerConnections.TryGetValue(ne.Entity.Id, out NetworkConnectionToClient connection))
            {
                NetworkServer.AddPlayerForConnection(connection, ne.Entity.gameObject);
            }
        }

        private void OnServerAddPlayer(object sender, GameEventArgs e)
        {
            ServerAddPlayerEventArgs ne = e as ServerAddPlayerEventArgs;
            if (ne == null) return;

            TbCharacterCfg tbCharacterCfg = GameEntry.LubanTable.GetTbCharacterCfg();
            CharacterCfg characterCfg = tbCharacterCfg.GetOrDefault(GameEntry.BigSpace.CharacterId);
            if (characterCfg == null)
            {
                Log.Error($"创建玩家失败：id {GameEntry.BigSpace.CharacterId} not found.");
                return;
            }
            
            int entityId = GameEntry.Entity.GenerateSerialId();
            GameEntry.Entity.ShowPlayer(new CharacterData(entityId, characterCfg.EntityId, CampType.Player, 1,
                characterCfg.Id));

            playerConnections.Add(entityId, ne.Connection);
        }
    }
}