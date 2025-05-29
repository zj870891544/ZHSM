using GameFramework.Event;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class MultiPlayerComponent : GameFrameworkComponent
    {
        [FormerlySerializedAs("networkManager")] [SerializeField] private MultiPlayerManager multiPlayerManager;

        private void Start()
        {
            GameEntry.Event.Subscribe(StartMultiPlayerEventArgs.EventId, OnStartMultiPlayer);
        }

        /// <summary>
        /// 启动/连接多人服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartMultiPlayer(object sender, GameEventArgs e)
        {
            StartMultiPlayerEventArgs ne = e as StartMultiPlayerEventArgs;
            if(ne == null) return;
            
            //检查端口范围
            if (ne.ServerPort >= ushort.MinValue && ne.ServerPort <= ushort.MaxValue && Transport.active is PortTransport portTransport)
            {
                portTransport.Port = (ushort)ne.ServerPort;
            }
            else
            {
                Log.Error($"multiplayer port {ne.ServerPort} is invalid.");
                return;
            }
            
            if (ne.IsHost)
            {
                multiPlayerManager.StartHost();
                GameEntry.Level.LoadLevel(GameEntry.Level.StartLevelId);
            }
            else
            {
                multiPlayerManager.networkAddress = ne.ServerIP;
                multiPlayerManager.StartClient();
            }
        }

        public void LoadScene(string sceneName)
        {
            if(NetworkServer.active)
                multiPlayerManager.ServerChangeScene(sceneName);
        }

        /// <summary>
        /// 完成加载场景回调
        /// </summary>
        public void FinishLoadScene()
        {
            multiPlayerManager.FinishLoadScene();
        }
    }
}