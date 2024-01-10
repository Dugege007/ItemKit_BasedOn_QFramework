using UnityEngine;

namespace LiveScriptReload.Runtime
{
    public class NetworkConnectionConfiguration: ScriptableObject
    {
        public int PortInUse = 5000;
        public bool IsUseSpecificIpAddressEnabled = false;
        public string UseSpecificIpAddress = "";


        public static NetworkConnectionConfiguration GetDefaultInstance()
        {
            return Resources.Load<NetworkConnectionConfiguration>("NetworkConnectionConfiguration");
        }
    }
}