using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GameFramework;
using GameFramework.Network;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;
using ZHSM.LogicServer;

namespace ZHSM
{
    public class BigSpaceComponent : GameFrameworkComponent
    {
        private const string LogicChannelName = "LogicServer";
        
        [Title("初始配置")] [PropertySpace]
        [SerializeField] private int characterId;

        public int CharacterId => int.Parse(m_LaunchData.jsonData.avatarResourceName);

        [SerializeField] private int m_B2LPlayerEntityId = 60004;
        [SerializeField] private LaunchData m_LaunchData;
        [SerializeField] private string m_CurrStage;
        [SerializeField] private string m_CurrArea;
        [SerializeField] private Vector3 m_PlayerPosition;
        [SerializeField] private Quaternion m_PlayerRotation;
        
        private INetworkChannel m_LogicChannel;
        private CancellationTokenSource m_Ctx;
        private Dictionary<string, B2LPlayerInfo> m_B2LPlayers = new Dictionary<string, B2LPlayerInfo>();

        private void Start()
        {
            m_Ctx = new CancellationTokenSource();
            m_LaunchData.playerOriginID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            
            new Thread(SendPlayerData).Start();
        }

        private void Update()
        {
            foreach (KeyValuePair<string, B2LPlayerInfo> kv in m_B2LPlayers)
            {
                B2LPlayerInfo b2LPlayerInfo = kv.Value;
                
                if (b2LPlayerInfo.Entity && !b2LPlayerInfo.Entity.CheckVisible())
                {
                    ReferencePool.Release(kv.Value);
                    m_B2LPlayers.Remove(kv.Key);
                    break;
                }
            }
        }

        private void OnDestroy()
        {
            m_Ctx.Cancel();
        }

        [HorizontalGroup]
        [Button(ButtonSizes.Large)]
        public void CreateLogicChannel()
        {
            m_LogicChannel = GameEntry.Network.CreateNetworkChannel(LogicChannelName, ServiceType.Tcp, new LogicChannelHelper());
            m_LogicChannel.HeartBeatInterval = 10.0f;
            m_LogicChannel.Connect(IPAddress.Parse(m_LaunchData.jsonData.serverIP), m_LaunchData.jsonData.serverPort);
        }

        [HorizontalGroup]
        [Button(ButtonSizes.Large)]
        public void RemoveChannel()
        {
            GameEntry.Network.DestroyNetworkChannel(LogicChannelName);
        }

        [PropertySpace(SpaceBefore = 20)]
        [Button("更新关卡进度", ButtonSizes.Large)]
        public void UpdateSeekInfo(string curStage, string curArea)
        {
            m_CurrStage = curStage;
            m_CurrArea = curArea;
        }

        [Button("更新玩家位置", ButtonSizes.Large)]
        public void UpdatePlayerInfo(Vector3 pos, Quaternion rot)
        {
            m_PlayerPosition = pos;
            m_PlayerRotation = rot;
        }

        private async void SendPlayerData()
        {
            while (!m_Ctx.IsCancellationRequested)
            {
                if (m_LogicChannel is not { Connected: true })
                {
                    await Task.Delay(1000, m_Ctx.Token);
                    
                    continue;
                }
            
                CSPlayerData playerData = ReferencePool.Acquire<CSPlayerData>();
                playerData.ID = m_LaunchData.playerOriginID;
                playerData.PrefabName = "CHR_Boy";
                playerData.PlayerNick = m_LaunchData.playerNick;

                playerData.Pos = ReferencePool.Acquire<FVector3>();
                playerData.Pos.x = m_PlayerPosition.x;
                playerData.Pos.y = m_PlayerPosition.y;
                playerData.Pos.z = m_PlayerPosition.z;

                playerData.Rotation = ReferencePool.Acquire<FVector4>();
                playerData.Rotation.x = m_PlayerRotation.x;
                playerData.Rotation.y = m_PlayerRotation.y;
                playerData.Rotation.z = m_PlayerRotation.z;
                playerData.Rotation.w = m_PlayerRotation.w;
            
                playerData.ContentID = m_LaunchData.contentID;
                playerData.TeamID = m_LaunchData.teamOriginID;
                playerData.TeamName = m_LaunchData.teamName;

                playerData.CurStage = m_CurrStage;
                playerData.CurArea = m_CurrArea;
                playerData.JsonData = "{}";

                m_LogicChannel.Send(playerData);
                
                await Task.Delay(33, m_Ctx.Token);
            }
        }

        public void AddB2LPlayerEntity(string playerId, B2LPlayerEntity playerEntity)
        {
            if (m_B2LPlayers.TryGetValue(playerId, out B2LPlayerInfo playerInfo))
            {
                playerInfo.Entity = playerEntity;
            }
            else
            {
                Log.Error("设置附近玩家实体失败 " + playerId);
            }
        }
        
        public void UpdateB2LPlayers(SCB2LPlayerData b2lPlayerData)
        {
            foreach (SyncPlayerData syncPlayer in b2lPlayerData.CollisionPlayers)
            {
                if (m_B2LPlayers.TryGetValue(syncPlayer.ID, out B2LPlayerInfo playerInfo))
                {
                    if (playerInfo.Entity)
                    {
                        playerInfo.Entity.UpdatePositionRotation(syncPlayer.Pos, syncPlayer.Rotation);
                    }
                }
                else
                {
                    int serialId = GameEntry.Entity.GenerateSerialId();
                    B2LPlayerInfo b2lPlayerInfo = ReferencePool.Acquire<B2LPlayerInfo>();
                    b2lPlayerInfo.PlayerId = syncPlayer.ID;
                    b2lPlayerInfo.EntityId = serialId;
                    m_B2LPlayers.Add(syncPlayer.ID, b2lPlayerInfo);
                    
                    GameEntry.Entity.ShowB2LPlayer(new B2LPlayerEntityData(serialId, m_B2LPlayerEntityId)
                    {
                        Position = new Vector3(syncPlayer.Pos.x, syncPlayer.Pos.y, syncPlayer.Pos.z),
                        Rotation = new Quaternion(syncPlayer.Rotation.x, syncPlayer.Rotation.y, syncPlayer.Rotation.z, syncPlayer.Rotation.z),
                        PlayerId = syncPlayer.ID,
                        RemoveDeley = 1.0f
                    });
                }
            }
        }
    }
}