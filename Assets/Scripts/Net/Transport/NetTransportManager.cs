using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

// Params:
// Ip address 
// connectionId
[Serializable] public class ConnectSucceededEvent : UnityEvent<string, int> { }
// Params:
// Ip address 
// Error
[Serializable] public class ConnectFailedEvent : UnityEvent<string, NetError> { }
// Params:
// connectionId
[Serializable] public class DiconnectedEvent : UnityEvent<int> { }
// Params:
// Packet
[Serializable] public class DataReceivedEvent : UnityEvent<NetTransportData> { }
// Params:
// Packet
[Serializable] public class BroadcastReceivedEvent : UnityEvent<NetTransportData> { }

/// <summary>
/// Manages the transport layer of the network.
/// Maintains a list of connections, and can send and receive data to them.
/// </summary>
public class NetTransportManager : MonoBehaviour 
{
    // Connections that are attempting to be made.
    public List<PendingConnection> PendingConnections = new List<PendingConnection>();

    public ConnectSucceededEvent ConnectSucceededEvent;
    public ConnectFailedEvent ConnectFailedEvent;
    public DiconnectedEvent DisconnectedEvent;
    public DataReceivedEvent DataReceivedEvent;
    public BroadcastReceivedEvent BroadcastReceivedEvent;

    // For debugging, all packets we ever received.
    public List<NetTransportData> AllReceivedData = new List<NetTransportData>();

    public const ushort MAX_PACKET_SIZE = 1500;

    private ConnectionConfig _config;
    private HostTopology _topology;
    private int _hostId;
    private bool _isInitialized;
    private bool _isOpen;
    private readonly Dictionary<NetQosType, int> _channelToId = new Dictionary<NetQosType, int>();
    private readonly Dictionary<int, NetQosType> _idToChannel = new Dictionary<int, NetQosType>();

    /// <summary>
    /// Open the connection.
    /// </summary>
    /// <param name="port">The port to open</param>
    /// <param name="numConnections">The max number of connections to this port.</param>
    /// <param name="channelTypes">The channel types to send/receive with.</param>
    public void Open(int port, int numConnections, params NetQosType[] channelTypes)
    {
        if(!_isOpen)
        {
            _isOpen = true;

            // Connections
            _config = new ConnectionConfig();
            _channelToId.Clear();
            _idToChannel.Clear();
            foreach(var channel in channelTypes)
            {
                byte channelId = _config.AddChannel(toUnetQosType(channel));
                _channelToId.Add(channel, channelId);
                _idToChannel.Add(channelId, channel);
            }

            // Define topology
            _topology = new HostTopology(_config, numConnections);

            // Open socket
            _hostId = NetworkTransport.AddHost(_topology, port);
        }
        else
        {
            Log.Warning(this, "You must close the transport layer before opening a new port.");
        }
    }

    public void Close()
    {
        if(_isOpen)
        {
            _isOpen = false;

            // Cleanup socket
            NetworkTransport.RemoveHost(_hostId);
        }
    }

    public void Connect(string ip, int port)
    {
        // EARLY OUT! //
        if (string.IsNullOrEmpty(ip))
        {
            Debug.LogError("Ip is null or empty.");
            return;
        }
        
        // Open socket
        byte responseCode;

        // Helper for testing, gets the local ip.
        if(ip == "localhost")
        {
            ip = getLocalIp();
        }
        
        // Note: 0 is the exception connection id, not sure if we will ever need that
        int connectionId = NetworkTransport.Connect(_hostId, ip, port, 0, out responseCode);

        if (isSuccessful(responseCode))
        {
            PendingConnections.Add(new PendingConnection(ip, connectionId));
        }
        else
        {
            handleError(responseCode);
            ConnectFailedEvent.Invoke(ip, toNetError(responseCode));
        }
    }

    public void Disconnect(int connectionId)
    {
        byte responseCode;
        NetworkTransport.Disconnect(_hostId, connectionId, out responseCode);

        if (isSuccessful(responseCode))
        {
            DisconnectedEvent.Invoke(connectionId);
        }
        else
        {
            // What to do if a disconnect fails?
            // Probably need to consider the connection severed anyway.
            handleError(responseCode);
        }
    }

    public void Send(int connectionId, byte[] data, int dataLength, NetQosType channelType)
    {
        if(_channelToId.ContainsKey(channelType))
        {
            int channelId = _channelToId[channelType];

            byte responseCode;
            NetworkTransport.Send(_hostId, connectionId, channelId, data, dataLength, out responseCode);

            if(!isSuccessful(responseCode))
            {
                // TODO: Do something when a send fails.
                handleError(responseCode);
            }
        }
        else
        {
            Log.Warning(this, "You cannot send to a channel that wasn't opened: " + channelType.ToString());
        }
    }

    private void Start()
    {
        _isInitialized = true;

        // Initialize transport layer
        GlobalConfig gConfig = new GlobalConfig();
        gConfig.MaxPacketSize = MAX_PACKET_SIZE;

        NetworkTransport.Init(gConfig);
        Log.Debug(this, "NetworkTransport initialized");
    }

    private void OnDestroy()
    {
        if(_isOpen)
        {
            Close();
        }

        if(_isInitialized)
        {
            _isInitialized = false;
            NetworkTransport.Shutdown();
            Log.Debug(this, "NetworkTransport shutdown");
        }
    }
	
	private void Update()
    {
        ReceiveData();
    }

    private void ReceiveData()
    {
        // Poll data received
        NetworkEventType eventType = NetworkEventType.Nothing;
        do
        {
            NetTransportData packet = new NetTransportData();
            eventType = NetworkTransport.Receive(out packet.RecHostId, out packet.ConnectionId, 
                out packet.ChannelId, packet.RecBuffer, MAX_PACKET_SIZE, 
                out packet.DataSize, out packet.ResponseCode);
            
            // Look up the channel type and add it to the packet info.
            if(_idToChannel.ContainsKey(packet.ChannelId))
            {
                packet.ChannelType = _idToChannel[packet.ChannelId];
            }
            
            switch(eventType)
            {
                case NetworkEventType.ConnectEvent:
                    AllReceivedData.Add(packet);
                    processConnectEvent(packet);
                    break;
                case NetworkEventType.DisconnectEvent:
                    AllReceivedData.Add(packet);
                    processDisconnectEvent(packet);
                    break;
                case NetworkEventType.DataEvent:
                    AllReceivedData.Add(packet);
                    processDataEvent(packet);
                    break;
                case NetworkEventType.BroadcastEvent:
                    AllReceivedData.Add(packet);
                    processBroadcastEvent(packet);
                    break;
                case NetworkEventType.Nothing:
                    // Do nothing.  We've processed all packets, and will now exit the do while.
                    break;
                default:
                    Debug.LogError("Unkown NetworkEventType: " + eventType.ToString());
                    break;
            }
        }
        while (eventType != NetworkEventType.Nothing);
    }

    /// <summary>
    /// Do stuff with the packet we received.
    /// </summary>
    /// <param name="packet">Precondition: Packet is not null.</param>
    private void processConnectEvent(NetTransportData packet)
    {
        logPacketReceived("processConnectEvent", packet);

        var pendingIndex = PendingConnections.FindIndex(p => p.ConnectionId == packet.ConnectionId);
        if(pendingIndex != -1)
        {
            var pending = PendingConnections[pendingIndex];
            PendingConnections.RemoveAt(pendingIndex);
            
            Log.Debug(this, "Connection attempt succeeded with id " + packet.ConnectionId);
            ConnectSucceededEvent.Invoke(pending.Ip, packet.ConnectionId);
        }
        else
        {
            Log.Debug(this, "A user has connected to us with id " + packet.ConnectionId);
            ConnectSucceededEvent.Invoke("", packet.ConnectionId);
        }
    }

    /// <summary>
    /// Do stuff with the packet we received.
    /// </summary>
    /// <param name="packet">Precondition: Packet is not null.</param>
    private void processDisconnectEvent(NetTransportData packet)
    {
        // TODO: This can both indicate a disconnect and a failed connection.  We need to bubble up more
        // info if it's the latter, like the network error and the ip that was trying to connect.
        logPacketReceived("processDisconnectEvent", packet);
        DisconnectedEvent.Invoke(packet.ConnectionId);
    }

    /// <summary>
    /// Do stuff with the packet we received.
    /// </summary>
    /// <param name="packet">Precondition: Packet is not null.</param>
    private void processDataEvent(NetTransportData packet)
    {
        logPacketReceived("processDataEvent", packet);
        DataReceivedEvent.Invoke(packet);
    }

    /// <summary>
    /// Do stuff with the packet we received.
    /// </summary>
    /// <param name="packet">Precondition: Packet is not null.</param>
    private void processBroadcastEvent(NetTransportData packet)
    {
        logPacketReceived("processBroadcastEvent", packet);
        BroadcastReceivedEvent.Invoke(packet);
    }

    /// <summary>
    /// Basic packet logging.
    /// </summary>
    private void logPacketReceived(string type, NetTransportData packet)
    {
        Log.Debug(this, string.Format("{0}\n{1}", type, packet.ToString()));
    }
    
    private bool isSuccessful(byte responseCode)
    {
        return (NetworkError)responseCode == NetworkError.Ok;
    }

    /// <summary>
    /// Generic erronous response code handling.
    /// </summary>
    private void handleError(byte responseCode)
    {
        if(!isSuccessful(responseCode))
        {
            // Just log it for now
            Log.Debug(this, "Error in packet: " + (NetworkError)responseCode);
        }
    }

    private NetError toNetError(byte responseCode)
    {
        return toNetError((NetworkError)responseCode);
    }

    // This requires dependencies to system.net, consider removing as it's just for testing.
    private static string getLocalIp()
    {
         IPHostEntry host;
         string localIp = "";
         host = Dns.GetHostEntry(Dns.GetHostName());
         foreach (IPAddress ip in host.AddressList)
         {
             if (ip.AddressFamily == AddressFamily.InterNetwork)
             {
                 localIp = ip.ToString();
                 break;
             }
         }
         return localIp;
    }

    /// <summary>
    /// Converts a Unet QosType to our own type.
    /// </summary>
    private static NetQosType toNetQosType(QosType qos)
    {
        return (NetQosType)(byte)qos;
    }

    /// <summary>
    /// Converts our QosType to Unet's.
    /// </summary>
    private static QosType toUnetQosType(NetQosType qos)
    {
        return (QosType)(byte)qos;
    }

    /// <summary>
    /// Converts a Unet NetworkErorr to our own NetError.
    /// </summary>
    private static NetError toNetError(NetworkError error)
    {
        return (NetError)(byte)error;
    }

    /// <summary>
    /// Converts our NetError to Unet's NetworkError.
    /// </summary>
    private static NetworkError toUnetNetworkError(NetError error)
    {
        return (NetworkError)(byte)error;
    }
}
