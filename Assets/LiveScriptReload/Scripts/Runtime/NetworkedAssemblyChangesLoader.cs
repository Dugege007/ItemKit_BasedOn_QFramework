using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

//Only included to make sure builds without LiveScriptReload_IncludeInBuild_Enabled will not throw MonoBehaviour serialization errors (if left in the scene)
#if !UNITY_EDITOR && !LiveScriptReload_IncludeInBuild_Enabled
namespace LiveScriptReload.Runtime
{
    public class NetworkedAssemblyChangesLoader : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField] private NetworkConnectionConfiguration _networkConnectionConfiguration;

        [UnityEngine.SerializeField] private bool _isDebug;
        [UnityEngine.SerializeField] private bool _editorActsAsRemoteClient;
    }
}
#else

using FastScriptReload.Runtime;
using LiteNetLib;
using ImmersiveVRTools.Runtime.Common.PropertyDrawer;

namespace LiveScriptReload.Runtime
{
    [PreventHotReload]
    [AddComponentMenu("Live Script Reload/Networked Assembly Changes Loader")]
    public class NetworkedAssemblyChangesLoader : MonoBehaviour, INetEventListener
    {
        private static readonly string LOG_PREFIX = $"[Live-Script-Reload Loader]: ";

        private NetManager _netClient;

        [SerializeField] private NetworkConnectionConfiguration _networkConnectionConfiguration;

        [SerializeField] private bool _isDebug;
        [SerializeField] [ShowIf(nameof(_isDebug))] private bool _editorActsAsRemoteClient;
        private NetworkConnectionKey _connectionKey;

        [RuntimeInitializeOnLoadMethod]
        static void AutoAddToLoadedScene()
        {
            if (!GameObject.FindObjectOfType<NetworkedAssemblyChangesLoader>())
            {
                var changesLoader = new GameObject("NetworkedAssemblyChangesLoader");
                changesLoader.AddComponent<NetworkedAssemblyChangesLoader>();
                DontDestroyOnLoad(changesLoader);
            }
        }

        void Start()
        {
            try
            {
                if(_isDebug) Debug.Log($"{LOG_PREFIX}Starting...");
                TryAutoPopulateNetworkConfig();
                _connectionKey = NetworkConnectionKey.Generate();
            }
            catch (Exception e)
            {
                Debug.LogError($"{LOG_PREFIX}Start failed, hot reload won't work. Error: {e}");
            }

#if UNITY_EDITOR
            if(!_isDebug || !_editorActsAsRemoteClient) {
                enabled = false;
                return;
            }
#endif
        }

        private void EnsureNetClientInit()
        {
            if (_netClient == null)
            {
                _netClient = new NetManager(this);
                _netClient.UnconnectedMessagesEnabled = true;
                _netClient.UpdateTime = 15;
                _netClient.Start();
            }
        }

#if UNITY_EDITOR
        void Reset()
        {
            TryAutoPopulateNetworkConfig();
        }
#endif

        private void TryAutoPopulateNetworkConfig()
        {
            if (_networkConnectionConfiguration == null)
            {
                if(_isDebug) Debug.Log($"{LOG_PREFIX}Loading Network Config...");
                _networkConnectionConfiguration = NetworkConnectionConfiguration.GetDefaultInstance();
                if(_isDebug) Debug.Log($"{LOG_PREFIX}Network Config Loaded...");
            }
        }
    
        void Update()
        {
            EnsureNetClientInit();
            
            _netClient.PollEvents();

            var peer = _netClient.FirstPeer;
            if (peer == null)
            {
                if (_networkConnectionConfiguration.IsUseSpecificIpAddressEnabled)
                {
                    Debug.Log($"{LOG_PREFIX} Trying to connect to specific address: {_networkConnectionConfiguration.UseSpecificIpAddress}:{_networkConnectionConfiguration.PortInUse}");
                    _netClient.Connect(_networkConnectionConfiguration.UseSpecificIpAddress, _networkConnectionConfiguration.PortInUse, _connectionKey.AsNetworkTransmittableString());
                }
                else
                {
                    _netClient.SendBroadcast(new byte[] {1}, _networkConnectionConfiguration.PortInUse);   
                }
            }
        }

        void OnDestroy()
        {
            if (_netClient != null)
                _netClient.Stop();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log($"{LOG_PREFIX}connected to editor on " + peer.EndPoint);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
            Debug.Log($"{LOG_PREFIX}received error " + socketErrorCode);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            Debug.Log($"{LOG_PREFIX}: received data, trying to get DLL contents");

            var dllData = reader.Get<DllData>();
            if (dllData.RawData.Length > 0)
            {
                Debug.Log($"{LOG_PREFIX}: Dll data parsed, loading to memory");
                var loadedAssembly = Assembly.Load(dllData.RawData);
                Debug.Log($"{LOG_PREFIX}: Dll loaded, redirecting calls");
                AssemblyChangesLoader.Instance.DynamicallyUpdateMethodsForCreatedAssembly(loadedAssembly, dllData.EditorOptions);
                Debug.Log($"{LOG_PREFIX}: Hot-reload finished");
            }
            else
            {
                Debug.LogWarning($"{LOG_PREFIX}Received data is not of {nameof(DllData)} type");
            }
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.BasicMessage && _netClient.ConnectedPeersCount == 0 && reader.GetInt() == 1)
            {
                Debug.Log($"{LOG_PREFIX}Received discovery response. Connecting to: " + remoteEndPoint);
                _netClient.Connect(remoteEndPoint, _connectionKey.AsNetworkTransmittableString());
            }
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
        
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log($"{LOG_PREFIX}[CLIENT] We disconnected because " + disconnectInfo.Reason);
        }
        

    }

    //connect by default supports just string, adding some additional data
    public class NetworkConnectionKey
    {
        private const string PropDelimiter = ":";
        private static readonly string[] PropDelimiterArr = new string[] { PropDelimiter };
        public string Key { get; }
        public string Device { get; }
        public bool IsEditor { get; }
        
        private NetworkConnectionKey(string key, string device, bool isEditor)
        {
            Key = key;
            Device = device;
            IsEditor = isEditor;
        }
        
        
        public static NetworkConnectionKey Generate()
        {
#if UNITY_EDITOR
            const bool isEditor = true;
#else
            const bool isEditor = false;
#endif
            return new NetworkConnectionKey($"LiveScriptReload-{Application.productName}", SystemInfo.deviceName, isEditor);
        }

        public string AsNetworkTransmittableString()
        {
            return Key + PropDelimiter + Device + PropDelimiter + IsEditor;
        }

        public static NetworkConnectionKey Parse(string networkTransmittableString)
        {
            try
            {
                var split = networkTransmittableString.Split(PropDelimiterArr, StringSplitOptions.RemoveEmptyEntries);
                return new NetworkConnectionKey(split[0], split[1], bool.Parse(split[2]));
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to parse connection key, {e}");
            }

            return null;
        }
    }
}
#endif