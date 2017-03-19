using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

// Just Saving for potential unit test 
/*
    public void TestSerialization()
    {
        IndexedPacket packet = new IndexedPacket();
        packet.Index = 5;
        packet.Data = new NetAtom[]
        {
            new ServerSpawnObject()
            {
                DataIndex = 1,
                PrefabName = "Yes",
                Position = 1,
                Scale =  2,
                Rotation = 3,
                ObjectId = 9,
                StartingReplicantId = 6
            },
            new ClientDestroyRequest()
            {
                DataIndex = 2,
                ObjectId = 3
            },
            new ClientSpawnRequest()
            {
                DataIndex = 3,
                PrefabName = "hi",
                Position = 4.15151f,
                Scale =  5,
                Rotation = 6
            }
        };

        BinarySerializer serializer = new BinarySerializer();
        var bytes = serializer.Serialize(packet);
        Debug.Log(string.Format("Packet serialized, {0} bytes", bytes.Length));
        RawPacket raw = new RawPacket()
        {
            RecBuffer = bytes,
            DataSize = bytes.Length
        };
        onDataReceived(raw);
    }

    private void onDataReceived(RawPacket packet)
    {
        if(packet != null)
        {
            BinarySerializer serializer = new BinarySerializer();
            var data = serializer.Deserialize(packet.RecBuffer, packet.DataSize) as IndexedPacket;
            if(data != null)
            {
                Debug.Log(data.ToString());
            }
        }
    }


    using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class NetConnectionManagerOld : MonoBehaviour 
{
    public int Port = 8888;
    public int MaxConnections = 4;
    public float TickRate = 1f / 30f;

    public UnityEvent PacketAcknowledgedEvent;

    // Our currently open connections.
    public List<Connection> Connections = new List<Connection>();
    private bool _isInitialized;
    private float _lastTick;
    private ISerializer _serializer;

    private static readonly QosType[] CHANNEL_TYPES = new QosType[] { QosType.Unreliable, QosType.Reliable };

    public void Init(ISerializer serializer)
    {
        _isInitialized = true;
        _serializer = serializer;

        var transport = SL.Get<NetTransportManager>();
        transport.Open(Port, MaxConnections, CHANNEL_TYPES);

        transport.ConnectSucceededEvent.AddListener(onConnectSucceeded);
        transport.ConnectFailedEvent.AddListener(onConnectFailed);
        transport.DisconnectedEvent.AddListener(onDisconnected);
        transport.DataReceivedEvent.AddListener(onDataReceived);
        transport.BroadcastReceivedEvent.AddListener(onBroadcastReceived);
    }


    public void Connect(string ip, int port)
    {
        SL.Get<NetTransportManager>().Connect(ip, port);
    }

    public void Disconnect(int connectionId)
    {
        SL.Get<NetTransportManager>().Disconnect(connectionId);
    }

    public void QueueAtom(int connectionId, QosType channelType, NetAtom atom)
    {
        var stream = getStream(connectionId, channelType);
        if(stream != null)
        {
            var buffer = stream._pendingAtoms;
            if(!buffer.IsFull)
            {
                buffer.PushBack(atom);
            }
            else
            {
                Log.Warning(this, "Buffer is full, connectionId = {0}, channelType = {1}", 
                    connectionId, channelType.ToString());
            }
        }
    }

    private void OnDestroy()
    {
        if(_isInitialized)
        {
            _isInitialized = false;
            var transport = SL.Get<NetTransportManager>();
            if(transport != null)
            {
                transport.ConnectSucceededEvent.RemoveListener(onConnectSucceeded);
                transport.ConnectFailedEvent.RemoveListener(onConnectFailed);
                transport.DisconnectedEvent.RemoveListener(onDisconnected);
                transport.DataReceivedEvent.RemoveListener(onDataReceived);
                transport.BroadcastReceivedEvent.RemoveListener(onBroadcastReceived);
        }
        }
    }

    private void Update()
    {
        if(_isInitialized)
        {
            if(_lastTick + TickRate < Time.time)
            {
                _lastTick = Time.time;

                tick();
            }
        }
    }

    private void tick()
    {
        foreach(var connection in Connections)
        {
            foreach(var streamKV in connection.Streams)
            {
                var stream = streamKV.Value;
                var pendingAtoms = stream._pendingAtoms;
                if(stream._outboundPackets.IsFull)
                {
                    Log.Warning(this, "Packet buffer is full!  We can't send more.  "
                        + "What do we do?  Disconnect from this client?");
                }
                else if(pendingAtoms.Length > 0)
                {
                    // Populate bytes with as many atoms as will fit in the packet.
                    byte[] buffer = new byte[IndexedPacket.MAX_PACKET_SIZE];
                    int index = stream.ClaimNextPacketIndex();
                    int size = fillPacket(index, pendingAtoms, ref buffer);

                    // Create a record of the packet being sent.
                    var record = new PacketRequest(index, buffer);
                    stream._outboundPackets.PushBack(record);

                    // Send the packet
                    send(connection.ConnectionId, buffer, streamKV.Key);
                }
            }
        }
    }

    /// <summary>
    /// Fills the packet, returns the size that it ended up being.
    /// /// PRECONDITION: packetId in the packet.
    /// </summary>
    /// <param name="packetId">The header.  Will be the first atom in the packet.</param>
    /// <param name="pending">The atoms to fill the packet with.</param>
    /// <param name="packetBytes">The bytes that will form the content</param>
    /// <returns>The number of bytes used.</returns>
    private static int fillPacket(int packetId, CircularBuffer<NetAtom> pending, ref byte[] packetBytes)
    {
        // EARLY OUT! //
        if(pending == null || packetBytes == null)
        {
            Log.Error(typeof(NetConnectionManager), "What the hell are you doing?");
            return 0;
        }

        // First attach the header (just the packet id for now)
        // Assume the header fits in the packet.
        byte[] headerBytes = BitConverter.GetBytes(packetId);
        int currentSize = headerBytes.Length;
        int maxSize = packetBytes.Length;
        Array.Copy(headerBytes, 0, packetBytes, currentSize, currentSize);

        // Essentially, move down the buffer and serialize until we run out of elements or the packet is full.
        while(!pending.IsEmpty)
        {
            var atom = pending.Front;

            //TODO: Use an in-place structure to avoid constant allocations
            byte[] bytesToAdd = _serializer.Serialize(atom);
            int sizeToAdd = bytesToAdd.Length;

            if(currentSize + sizeToAdd < maxSize)
            {
                // We've determined we have space, pop the atom off.
                // Wait what are you talking about?  We want to keep sending until they are received.
                // But only for unreliable packets right? 
                // I have to think about if this method is common to all channels or just for the
                // unreliable one.
                //pending.PopFront();
                currentSize += sizeToAdd;
                Array.Copy(bytesToAdd, 0, packetBytes, currentSize, sizeToAdd);
            }
            else
            {
                // The packet is as full as we could make it.  
                // This could leave quite a bit of empty space at the end if our last atom is quite big, 
                // but let's optimize once we're sure we need to.
                break;
            }
        }

        return currentSize;
    }

    private void send(int connectionId, byte[] bytes, QosType channelType)
    {
        SL.Get<NetTransportManager>().Send(connectionId, bytes, channelType);
    }

    private NetChannelStream getStream(int connectionId, QosType channelType)
    {
        NetChannelStream stream = null;
        var connection = getConnection(connectionId);
        if(connection != null)
        {
            stream = connection.Streams[channelType];
        }

        return stream;
    }

    private Connection getConnection(int connectionId)
    {
        return Connections.Find(p => p.ConnectionId == connectionId);
    }

#region Event Listeners
    private void onConnectSucceeded(string ip, int connectionId)
    {
        var connection = new Connection(ip, connectionId, CHANNEL_TYPES);
    }

    private void onConnectFailed(string ip, NetworkError networkError)
    {
        // Passthrough
    }

    private void onDataReceived(RawPacket packet)
    {
        // EARLY OUT! //
        if(packet == null)
        {
            Log.Warning(this, "Why is a packet null?");
            return;
        }

        // Process ack of a packet (throw away all deltas before that)
        // Process data received for game (GameData?)
        var data = packet.RecBuffer;
        var packetIndex = BitConverter.ToInt32(data, 0);
        //object data = 

    }

    private void onDisconnected(int connectionId)
    {
        var index = Connections.FindIndex(p => p.ConnectionId == connectionId);
        if (index != -1)
        {
            Connections.RemoveAt(index);
        }
        
        // Notify listeners.
    }

    private void onBroadcastReceived(RawPacket packet)
    {
        throw new NotImplementedException();
    }
#endregion
}





// Differences between clients and servers
// Servers can send out their data to clients.
// Clients can only send out requests to server.
// Server dispatches some data out to all clients.
// All machines want to know what the latest packet received was.

public class PacketRequest
{
    public int Index;
    public byte[] Bytes;

    public PacketRequest(int index, byte[] bytes)
    {
        Index = index;
        Bytes = bytes;
    }
}

[System.Serializable]
public class IndexedPacket
{
    public const int MAX_PACKET_SIZE = 1500;
    public int Index;
    public NetAtom[] Data;

    public override string ToString ()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Packet {0}, {1} entries\n", this.Index, this.Data.Length);
        foreach(var element in this.Data)
        {
            sb.AppendFormat("{0}\n", element.GetType().ToString());
            sb.AppendFormat(": {0}\n", element.ToString());
        }
        return sb.ToString();
    }
}

[System.Serializable]
public class NetClientData : NetAtom
{

}

[System.Serializable]
public class ClientSpawnRequest: NetClientData
{
    public string PrefabName;
    public float Position;
    public float Scale;
    public float Rotation;

    public override string ToString()
    {
        return PrefabName + Position + Scale + Rotation;
    }
}

[System.Serializable]
public class ClientDestroyRequest: NetClientData
{
    public int ObjectId;
}

[System.Serializable]
public class NetServerData : NetAtom
{
    
}

[System.Serializable]
public class ServerSpawnObject : NetServerData
{
    public string PrefabName;
    public float Position;
    public float Scale;
    public float Rotation;
    public int ObjectId;
    public int StartingReplicantId;
}



public class User
{
    
}
*/