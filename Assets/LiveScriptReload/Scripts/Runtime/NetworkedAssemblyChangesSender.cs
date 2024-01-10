using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using UnityEngine;

//Only included to make sure builds without LiveScriptReload_IncludeInBuild_Enabled will not throw MonoBehaviour serialization errors (if left in the scene)
#if !UNITY_EDITOR && !LiveScriptReload_IncludeInBuild_Enabled
namespace LiveScriptReload.Runtime
{
    public class NetworkedAssemblyChangesSender: MonoBehaviour
    {
        [UnityEngine.SerializeField] private NetworkConnectionConfiguration _networkConnectionConfiguration;
    }
}

#else

using FastScriptReload.Runtime;
using LiteNetLib;
using LiteNetLib.Utils;

namespace LiveScriptReload.Runtime
{
    [PreventHotReload]
    public class NetworkedAssemblyChangesSender : SingletonBase<NetworkedAssemblyChangesSender>, IAssemblyChangesLoader 
#if UNITY_EDITOR
    , INetEventListener, INetLogger
#endif
    {
        private const string LOG_PREFIX = "[Live-Script-Reload-Sender]: ";
#if UNITY_EDITOR
    private NetManager _netServer;
    private NetPeer _connectedPeer;
    private NetDataWriter _dataWriter;
    private string _requireConnectionKey;

    [SerializeField] private NetworkConnectionConfiguration _networkConnectionConfiguration;

    public NetPeer ConnectedPeer
    {
        get => _connectedPeer;
        set
        {
            _connectedPeer = value;
            if (value == null)
            {
                ConnectedClientDetails = null;
            }
        }
    }

    public NetworkConnectionKey ConnectedClientDetails { get; private set; }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        NetDebug.Logger = this;
        _dataWriter = new NetDataWriter();
        _netServer = new NetManager(this);
        TryAutoPopulateNetworkConfig();
        _netServer.Start(_networkConnectionConfiguration.PortInUse);
        if (!_networkConnectionConfiguration.IsUseSpecificIpAddressEnabled)
        {
            _netServer.BroadcastReceiveEnabled = true;
        }

        _netServer.UpdateTime = 15;

        _requireConnectionKey = NetworkConnectionKey.Generate().Key;
    }
    
    void Update()
    {
        _netServer.PollEvents();
    }
    
    void Reset()
    {
        TryAutoPopulateNetworkConfig();
    }

    private void TryAutoPopulateNetworkConfig()
    {
        if (_networkConnectionConfiguration == null)
        {
            _networkConnectionConfiguration = NetworkConnectionConfiguration.GetDefaultInstance();
        }
    }

    public void DynamicallyUpdateMethodsForCreatedAssembly(Assembly dynamicallyLoadedAssemblyWithUpdates, AssemblyChangesLoaderEditorOptionsNeededInBuild editorOptions)
    {
        if (ConnectedPeer != null)
        {
            _dataWriter.Reset();
            _dataWriter.Put(new DllData(File.ReadAllBytes(dynamicallyLoadedAssemblyWithUpdates.Location), editorOptions));
            
            ConnectedPeer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
        }
        else
        {
            Debug.LogWarning($"{LOG_PREFIX}No client connected, changes will not be send (editor will still Hot-Reload).");
        }
        
        if (ConnectedClientDetails == null || !ConnectedClientDetails.IsEditor)
        {
            AssemblyChangesLoader.Instance.DynamicallyUpdateMethodsForCreatedAssembly(dynamicallyLoadedAssemblyWithUpdates, editorOptions);
        }
    }
    void OnDestroy()
    {
        NetDebug.Logger = null;
        if (_netServer != null)
            _netServer.Stop();
    }

    public void OnPeerConnected(NetPeer peer)
    {
        //TODO: when multiple clients connect, force disconnect, allow just 1
        Debug.Log($"{LOG_PREFIX}New device connected ({ConnectedClientDetails?.Device}{(ConnectedClientDetails?.IsEditor ?? false ? " [Editor]" : "")}) from " + peer.EndPoint);
        ConnectedPeer = peer;
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.Log($"{LOG_PREFIX}Network error " + socketErrorCode);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.Broadcast)
        {
            if (!_networkConnectionConfiguration.IsUseSpecificIpAddressEnabled)
            {
                Debug.Log($"{LOG_PREFIX}Received discovery request. Send discovery response");
                var resp = new NetDataWriter();
                resp.Put(1);
                _netServer.SendUnconnectedMessage(resp, remoteEndPoint);
            }
            else
            {
                Debug.LogWarning($"{LOG_PREFIX}Received discovery request but preferences specify to use fixed IP Address, please make sure client configuration is correct, connection won't be made.");
            }
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        try
        {
            var passedKeyString = request.Data.GetString();
            var passedKey = NetworkConnectionKey.Parse(passedKeyString);
            if (passedKey != null)
            {
                if (passedKey.Key ==  _requireConnectionKey)
                {
                    if (ConnectedClientDetails != null)
                    {
                        Debug.Log($"{LOG_PREFIX}Connection request from: {passedKey.Device} ({request.RemoteEndPoint}) - rejecting as client already connected.");
                        request.Reject();
                        return;
                    }
                
                    ConnectedClientDetails = passedKey;
                    request.Accept();
                }
                else
                {
                    Debug.LogWarning($"{LOG_PREFIX}Connection request from: {passedKey.Device} ({request.RemoteEndPoint}) - for different editor instance: '{passedKey.Key}'. Connection rejected.");
                    request.Reject();
                }
            }
            else
            {
                Debug.LogError($"{LOG_PREFIX}Connection request from: {passedKey.Device} ({request.RemoteEndPoint}) - no valid key present. Please contact support");
            }
        }
        catch
        {
            Debug.LogError($"{LOG_PREFIX}Invalid incoming connection data");
        }
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log($"{LOG_PREFIX}client disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
        if (peer == ConnectedPeer)
            ConnectedPeer = null;
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
    }

    public void WriteNet(NetLogLevel level, string str, params object[] args)
    {
        Debug.LogFormat(str, args);
    }
#else
        public void DynamicallyUpdateMethodsForCreatedAssembly(Assembly dynamicallyLoadedAssemblyWithUpdates, AssemblyChangesLoaderEditorOptionsNeededInBuild editorOptions) {
            throw new Exception("Shouldn't be called in non-editor workflow");
        }

#endif
    }
    
    [Serializable]
    internal struct DllData: INetSerializable
    {
        public byte[] RawData;
        public AssemblyChangesLoaderEditorOptionsNeededInBuild EditorOptions;

        public DllData(byte[] rawData, AssemblyChangesLoaderEditorOptionsNeededInBuild editorOptions)
        {
            RawData = rawData;
            EditorOptions = editorOptions;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutBytesWithLength(RawData);
            
            writer.Put(EditorOptions.EnableExperimentalAddedFieldsSupport);
            writer.Put(EditorOptions.IsDidFieldsOrPropertyCountChangedCheckDisabled);
        }

        public void Deserialize(NetDataReader reader)
        {
            RawData = reader.GetBytesWithLength();

            EditorOptions = new AssemblyChangesLoaderEditorOptionsNeededInBuild(
                reader.GetBool(),
                reader.GetBool()
            );
        }
    }
}
#endif