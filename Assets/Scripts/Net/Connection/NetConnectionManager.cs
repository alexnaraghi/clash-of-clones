using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// Params:
// Atom
// Channel
// Connection
[Serializable] public class AtomEvent : UnityEvent<NetAtom, NetQosType, NetConnection> { }
[Serializable] public class ConnectedEvent : UnityEvent<NetConnection> { }
[Serializable] public class ConnectedFailedEvent : UnityEvent<string> { }
[Serializable] public class DisconnectedEvent : UnityEvent<NetConnection> { }

public class NetConnectionManager : MonoBehaviour
{
    public int MaxConnections = 4;
    public int MaxAtomSize = 1024;

    [SerializeField] [Readonly] private int _myPort;
    [SerializeField] [Readonly] private bool _isOpened;

    public ConnectedEvent ConnectedEvent;
    public ConnectedFailedEvent ConnectedFailedEvent;
    public DisconnectedEvent DiconnectedEvent;
    public AtomEvent AtomReceivedEvent;

    // ReceivedAtoms list is for testing.
    public List<NetAtom> ReceivedAtoms = new List<NetAtom>();
    public CircularBuffer<NetPodTransmissionInfo> OutboundPods = new CircularBuffer<NetPodTransmissionInfo>(128);
    public readonly List<NetConnection> Connections = new List<NetConnection>();

    private ISerializer _serializer;
    private int _podIndex;

    public bool IsOpen
    {
        get
        {
            return _isOpened;
        }
    }

    public void Init(ISerializer serializer)
    {
        _serializer = serializer;
    }

    public void Open(int myPort, NetQosType[] channels)
    {
        if (!_isOpened)
        {
            _isOpened = true;
            _myPort = myPort;

            var transport = SL.Get<NetTransportManager>();
            transport.Open(_myPort, MaxConnections, channels);

            transport.ConnectSucceededEvent.AddListener(onConnectSucceeded);
            transport.ConnectFailedEvent.AddListener(onConnectFailed);
            transport.DisconnectedEvent.AddListener(onDisconnected);
            transport.DataReceivedEvent.AddListener(onDataReceived);
            transport.BroadcastReceivedEvent.AddListener(onBroadcastReceived);

            Log.Debug(this, "Connection opened on port " + myPort);
        }
        else
        {
            Log.Debug(this, "You can't open more than one port currently.");
        }
    }

    public void Close()
    {
        if (_isOpened)
        {
            _isOpened = false;

            Log.Debug(this, "Connection closed on port " + _myPort);

            // This can happen during OnDestroy, make sure stuff exists.
            if (SL.Exists && SL.Get<NetTransportManager>())
            {
                SL.Get<NetTransportManager>().Close();
                SL.Get<NetTransportManager>().ConnectSucceededEvent.RemoveListener(onConnectSucceeded);
                SL.Get<NetTransportManager>().ConnectFailedEvent.RemoveListener(onConnectFailed);
                SL.Get<NetTransportManager>().DisconnectedEvent.RemoveListener(onDisconnected);
                SL.Get<NetTransportManager>().DataReceivedEvent.RemoveListener(onDataReceived);
                SL.Get<NetTransportManager>().BroadcastReceivedEvent.RemoveListener(onBroadcastReceived);
            }
        }
    }

    public void Connect(string ip, int port)
    {
        SL.Get<NetTransportManager>().Connect(ip, port);
        Log.Debug(this, "Connecting to {0}:{1}", ip, port);
    }

    public void Disconnect(NetConnection connection)
    {
        if (connection != null)
        {
            SL.Get<NetTransportManager>().Disconnect(connection.ConnectionId);
            Log.Debug(this, "Disconnecting from {0}", connection.Ip);
        }
    }

    public bool IsConnected(NetConnection connection)
    {
        return Connections.Contains(connection);
    }

    public void Send(NetConnection connection, NetQosType channelType, NetAtom atom)
    {
        if(connection != null)
        {
            var transmissionInfo = createTransmission(connection.ConnectionId, channelType, atom);
            if (transmissionInfo != null)
            {
                byte[] buffer = new byte[MaxAtomSize];
                int length = _serializer.Serialize(transmissionInfo.Pod, buffer);
                SL.Get<NetTransportManager>().Send(transmissionInfo.ConnectionId, buffer, length, transmissionInfo.ChannelType);

                OutboundPods.PushBack(transmissionInfo);
            }
            else
            {
                Log.Error(this, "Null transmission info on send, info index is {0}", transmissionInfo.Pod.Index);
            }
        }
    }

    private void OnDestroy()
    {
        if (_isOpened)
        {
            Close();
        }
    }

    /// <summary>
    /// Creates a record of transmission for a net atom.
    /// </summary>
    private NetPodTransmissionInfo createTransmission(int connectionId, NetQosType channelType, NetAtom atom)
    {
        // EARLY OUT! //
        if (atom == null) return null;

        var index = _podIndex++;
        var pod = new NetAtomPod() { Index = index, Content = atom };
        return new NetPodTransmissionInfo(connectionId, channelType, pod);
    }

    #region Event Listeners
    private void onConnectSucceeded(string ip, int connectionId)
    {
        Log.Debug(this, "Connect succeeded to {0}, id is {1}", ip, connectionId);

        var connection = new NetConnection(ip, connectionId);
        Connections.Add(connection);
        ConnectedEvent.Invoke(connection);
    }

    private void onConnectFailed(string ip, NetError networkError)
    {
        Log.Debug(this, "Connect failed to {0}, error {1}", ip, networkError.ToString());

        // Passthrough
        ConnectedFailedEvent.Invoke(ip);
    }

    private void onDataReceived(NetTransportData packet)
    {
        // EARLY OUT! //
        if (packet == null)
        {
            Log.Warning(this, "Why is a packet null?");
            return;
        }

        NetPod pod = _serializer.Deserialize(packet.RecBuffer, 0, packet.DataSize) as NetPod;
        if (pod != null)
        {
            if (pod is NetAtomPod)
            {
                var atomPod = pod as NetAtomPod;
                var atom = atomPod.Content;
                if(atom != null)
                {
                    ReceivedAtoms.Add(atom);

                    var connection = getConnection(packet.ConnectionId);
                    if (connection != null)
                    {
                        AtomReceivedEvent.Invoke(atom, packet.ChannelType, connection);
                    }
                }
                else
                {
                    Log.Error(this, "Null atom in pod.");
                }
            }
            else
            {
                Log.Error(this, "Unknown type of pod: " + pod.GetType());
            }
        }
        else
        {
            Log.Warning(this, "Unparsable data of size {0}", packet.DataSize);
        }
    }

    private void onDisconnected(int connectionId)
    {
        var connection = getConnection(connectionId);
        if (connection != null)
        {
            Log.Debug(this, "Disconnected from {0}, id {1}", connection.Ip, connectionId);

            Connections.Remove(connection);
            DiconnectedEvent.Invoke(connection);
        }
        else
        {
            // TODO: Figure out how to get the ip.
            ConnectedFailedEvent.Invoke(connectionId.ToString());
            Log.Debug(this, "Connect failed, id {0}", connectionId);
        }
    }

    private NetConnection getConnection(int connectionId)
    {
        NetConnection connection = null;
        var index = Connections.FindIndex(p => p.ConnectionId == connectionId);
        if (index != -1)
        {
            connection = Connections[index];
        }
        return connection;
    }

    private void onBroadcastReceived(NetTransportData packet)
    {
        throw new NotImplementedException();
    }
    #endregion
}