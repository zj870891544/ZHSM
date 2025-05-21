using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class MultiPlayerComponent : GameFrameworkComponent
    {
        [FormerlySerializedAs("networkManager")] [SerializeField] private MultiPlayerManager multiPlayerManager;

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

        /// <summary>
        /// 开启服务器
        /// </summary>
        public void StartHost()
        {
            multiPlayerManager.StartHost();
        }

        /// <summary>
        /// 停止服务器
        /// </summary>
        public void StopHost()
        {
            multiPlayerManager.StopHost();
        }

        public void StartClient()
        {
            multiPlayerManager.networkAddress = GameEntry.BigSpace.Host;
            if (Transport.active is PortTransport portTransport)
            {
                portTransport.Port = GameEntry.BigSpace.Port;
            }
            
            multiPlayerManager.StartClient();
        }

        public void StopClient()
        {
            multiPlayerManager.StopClient();
        }
    }
}