using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections.Generic;

/*
/// <summary>
/// From tribes networking
/// the frequency of packet transmission as well
/// as packet size and stream manager ordering
/// </summary>
public class StreamManager : MonoBehaviour 
{
    /// <summary>
    /// Data to be sent to the connections.
    /// </summary>
    private Dictionary<Connection, Queue<object>> _queuedData = new Dictionary<Connection, Queue<object>>();

    public void Queue(Connection connection, object content)
    {
        if(connection != null && content != null)
        {
            _queuedData[connection].Enqueue(content);
        }
        else
        {
            Debug.LogError("Queueing null connection or content");
        }
    }

    private void onConnected(Connection connnection)
    {
        _queuedData.Add(connnection, new Queue<object>());
    }

    private void onDisconnected(Connection connection)
    {
        _queuedData.Remove(connection);
    }

    /// <summary>
    /// Manage packet sending
    /// </summary>
    private void Update()
    {
        // Fill packets with queued data.
        foreach(var connectionKV in _queuedData)
        {
            var packet = fillPacket(connectionKV.Value);

            if(packet.Length > 0)
            {
                sendPacket(connectionKV.Key, packet);
            }
        }
    }

    private byte[] fillPacket(Queue<object> queue)
    {
        int size = 0;
        List<byte[]> packetData = new List<byte[]>();

        // Assumes that the first object in the queue is smaller than the max packet size.
        while(queue.Count > 0)
        {
            var current = queue.Peek();
            BinarySerializer serializer = new BinarySerializer();
            byte[] bytes = serializer.Serialize(current);

            if(size + bytes.Length < RawPacket.BUFFER_SIZE)
            {
                size += bytes.Length;
                packetData.Add(bytes);
                queue.Dequeue();
            }
            else
            {
                // The packet is full.
                Debug.Log("Packet was full, entire data set could not be sent.");
                break;
            }
        }

        // TODO: Concatenate all byte arrays and return the full packet.
        return new byte[0];
    }

    private void sendPacket(Connection connection, byte[] bytes)
    {
        var transport = SL.Get<NetTransportManager>();
        transport.Send(connection, bytes, isReliable: false);
    }
}

 */