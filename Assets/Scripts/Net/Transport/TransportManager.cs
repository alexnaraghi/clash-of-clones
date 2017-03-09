using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Manages the transport layer of the network.
/// Maintains a list of connections, and can send and receive data to them.
/// </summary>
public class TransportManager : MonoBehaviour 
{
    public int Port = 8888;
    public int NumConnections = 4;

    // For debugging, all packets we ever received.
    public List<RawPacket> AllPackets = new List<RawPacket>();
    // Our currently open connections.
    public List<Connection> Connections = new List<Connection>();

    private ConnectionConfig _config;
    private HostTopology _topology;
    private int _reliableChannelId;
    private int _unreliableChannelId;
    private int _hostId;

    public void Connect(string ip)
    {
        // EARLY OUT! //
        if (string.IsNullOrEmpty(ip))
        {
            Debug.LogError("Ip is null or empty.");
            return;
        }
        
        // Open socket
        byte responseCode;
        
        // Note: 0 is the exception connection id, not sure if we will ever need that
        int connectionId = NetworkTransport.Connect(_hostId, ip, Port, 0, out responseCode);

        if(!isSuccessful(responseCode))
        {
            handleError(responseCode);
        }
    }

    public void Disconnect(Connection connection)
    {
        // EARLY OUT! //
        if (connection == null)
        {
            Debug.LogError("Connection is null.");
            return;
        }

        byte responseCode;
        NetworkTransport.Disconnect(_hostId, connection.ConnectionId, out responseCode);

        if(!isSuccessful(responseCode))
        {
            handleError(responseCode);
        }
    }

    public void Send(Connection connection, byte[] data, bool isReliable)
    {
        // EARLY OUT! //
        if (connection == null)
        {
            Debug.LogError("Connection is null.");
            return;
        }

        int channelId = isReliable ? _reliableChannelId : _unreliableChannelId;
        byte responseCode;
        NetworkTransport.Send(_hostId, connection.ConnectionId, channelId, data, data.Length, out responseCode);

        if(!isSuccessful(responseCode))
        {
            handleError(responseCode);
        }
    }

    // Use this for initialization
    private void Start () 
    {
        // Initialize transport layer
        // If you need to configure the network with custom settings
        //GlobalConfig gConfig = new GlobalConfig();
        //gConfig.MaxPacketSize = 500;
        NetworkTransport.Init();

        // Connections
        _config = new ConnectionConfig();
        _reliableChannelId = _config.AddChannel(QosType.Reliable);
        _unreliableChannelId = _config.AddChannel(QosType.Unreliable);

        // Define topology
        _topology = new HostTopology(_config, NumConnections);

        // Open socket
        _hostId = NetworkTransport.AddHost(_topology, Port);
	}

    private void OnDestroy()
    {
        // Cleanup socket
        NetworkTransport.RemoveHost(_hostId);
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
            RawPacket packet = new RawPacket();
            eventType = NetworkTransport.Receive(out packet.RecHostId, out packet.ConnectionId, 
                out packet.ChannelId, packet.RecBuffer, RawPacket.BUFFER_SIZE, 
                out packet.DataSize, out packet.ResponseCode);
            
            switch(eventType)
            {
                case NetworkEventType.ConnectEvent:
                    AllPackets.Add(packet);
                    processConnectEvent(packet);
                    break;
                case NetworkEventType.DisconnectEvent:
                    AllPackets.Add(packet);
                    processDisconnectEvent(packet);
                    break;
                case NetworkEventType.DataEvent:
                    AllPackets.Add(packet);
                    processDataEvent(packet);
                    break;
                case NetworkEventType.BroadcastEvent:
                    AllPackets.Add(packet);
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
    private void processConnectEvent(RawPacket packet)
    {
        logPacketReceived("processConnectEvent", packet);

        // Add to the connection list
        var connection = new Connection() 
        { 
            ConnectionId = packet.ConnectionId 
        };
        Connections.Add(connection);
    }

    /// <summary>
    /// Do stuff with the packet we received.
    /// </summary>
    /// <param name="packet">Precondition: Packet is not null.</param>
    private void processDisconnectEvent(RawPacket packet)
    {
        logPacketReceived("processDisconnectEvent", packet);

        // Remove from the connection list
        int index = Connections.FindIndex(c => c.ConnectionId == packet.ConnectionId);
        if(index != -1)
        {
            Connections.RemoveAt(index);
        }
    }

    /// <summary>
    /// Do stuff with the packet we received.
    /// </summary>
    /// <param name="packet">Precondition: Packet is not null.</param>
    private void processDataEvent(RawPacket packet)
    {
        logPacketReceived("processDataEvent", packet);
    }

    /// <summary>
    /// Do stuff with the packet we received.
    /// </summary>
    /// <param name="packet">Precondition: Packet is not null.</param>
    private void processBroadcastEvent(RawPacket packet)
    {
        logPacketReceived("processBroadcastEvent", packet);
    }

    /// <summary>
    /// Basic packet logging.
    /// </summary>
    private void logPacketReceived(string type, RawPacket packet)
    {
        Debug.Log(string.Format("{0}\n{1}", type, packet.ToString()));
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
            Debug.Log("Error in packet: " + (NetworkError)responseCode);
        }
    }
}
