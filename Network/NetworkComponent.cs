using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public class NetworkComponent : GameFrameworkComponent
    {
        [SerializeField] private NetworkManager networkManager;

        public void LoadScene(string sceneName)
        {
            if(NetworkServer.active)
                networkManager.ServerChangeScene(sceneName);
        }

        /// <summary>
        /// 完成加载场景回调
        /// </summary>
        public void FinishLoadScene()
        {
            networkManager.FinishLoadScene();
        }

        /// <summary>
        /// 开启服务器
        /// </summary>
        public void StartHost()
        {
            networkManager.StartHost();
        }

        /// <summary>
        /// 停止服务器
        /// </summary>
        public void StopHost()
        {
            networkManager.StopHost();
        }

        public void StartClient()
        {
            networkManager.networkAddress = GameEntry.BigSpace.Host;
            if (Transport.active is PortTransport portTransport)
            {
                portTransport.Port = GameEntry.BigSpace.Port;
            }
            
            networkManager.StartClient();
        }

        public void StopClient()
        {
            networkManager.StopClient();
        }
    }
}