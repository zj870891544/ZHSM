using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace ZHSM
{
    public static class SystemUtility
    {
        public static string GetDeviceUniqueIdentifier()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }

        public static string GetIpAddress()
        {
            string ipAddress = string.Empty;
            
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip.ToString();
                }
            }

            return ipAddress;
        }
    }
}