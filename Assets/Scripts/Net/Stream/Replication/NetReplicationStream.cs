using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetReplicationStream : MonoBehaviour, INetStream
{
    public NetQosType[] ReceiveChannelTypes = new NetQosType[] { NetQosType.Unreliable, NetQosType.Reliable };
    public NetQosType[] AckChannelTypes = new NetQosType[] { NetQosType.Unreliable };

    private List<NetBehaviourBase> _replicants = new List<NetBehaviourBase>();
    private int _latestId = 0;
    //public readonly Dictionary<NetQosType, NetChannel> Channels = new Dictionary<NetQosType, NetChannel>();

    // For testing, sample and apply everything in the game.
    private List<NetReplicationDelta> _snapshot = new List<NetReplicationDelta>();

    public NetQosType[] RequiredChannels
    {
        get
        {
            return new NetQosType[] { NetQosType.Unreliable };
        }
    }

    public void Spawn(GameObject go)
    {
        var components = go.GetInterfaces<NetBehaviourBase>();
        foreach(var component in components)
        {
            assignId(component);
            _replicants.Add(component);
        }

        // TODO: spawn this to on all clients.
    }

    public void Despawn(GameObject go)
    {
        var components = go.GetInterfaces<NetBehaviourBase>();
        foreach(var component in components)
        {
            _replicants.Remove(component);
        }
    }

    public void TakeSnapshot()
    {
        _snapshot.Clear();
        foreach(var component in _replicants)
        {
            var content = component.Sample();
            var sample = new NetReplicationDelta(component.Id, content);
            _snapshot.Add(sample);
        }
    }

    public void ApplySnapshot()
    {
        foreach(var sample in _snapshot)
        {
            var component = _replicants.Find(f => f.Id == sample.ReplicantId);
            if(component != null)
            {
                component.Apply(sample.Content);
            }
        }
    }

    public void EnableStream()
    {

    }

    public void DisableStream()
    {

    }

    private void Update()
    {
        // Do a tick rate.
        // Replication uses unreliable packets so it's unique.
        // It needs to use acks
        // And it has its own solution for determining how to form a packet that contains the most
        // up to date information.

        // Only update the replicants due for an update.
        // Don't update replicants if they acknowledged an update recently.
    }

    private void assignId(NetBehaviourBase component)
    {
        component.Init(_latestId++);
    }
}



/*


    
    /// <summary>
    /// Any channels that we are receiving data or acks on should be opened for business.
    /// Add net channels to the channel dictionary.
    /// </summary>
    private static void registerChannels(Dictionary<NetQosType, NetChannel> channels, NetQosType[] receives, NetQosType[] acks)
    {
        foreach (var channelTypeObj in Enum.GetValues(typeof(NetQosType)))
        {
            var channelType = (NetQosType)channelTypeObj;
            bool shouldReceive = receives.Contains(channelType);
            bool shouldAck = acks.Contains(channelType);

            if(shouldReceive || shouldAck)
            {
                channels.Add(channelType, new NetChannel(shouldReceive, shouldAck));
            }
        }
    }

[Serializable]
public class NetChannel
{
    public readonly bool ShouldReceive;
    public readonly bool ShouldAck;
    public readonly AtomEvent AtomReceivedEvent;
    public readonly AtomAckEvent AtomAckEvent;

    public NetChannel(bool shouldReceive, bool shouldAck)
    {
        ShouldReceive = shouldReceive;
        ShouldAck = shouldAck;
        AtomReceivedEvent = new AtomEvent();
        AtomAckEvent = new AtomAckEvent();
    }
}

// Params:
// ConnectionId
// Atom
[Serializable] public class AtomAckEvent : UnityEvent<int, NetAtom> { }

private void onDataReceived(NetTransportData packet)
    {
        // EARLY OUT! //
        if (packet == null)
        {
            Log.Warning(this, "Why is a packet null?");
            return;
        }

        // EARLY OUT! //
        if (!Channels.ContainsKey(packet.ChannelType))
        {
            Log.Debug(this, "Received packet on unregistered channel: " + packet.ChannelType);
            return;
        }

        NetPod pod = _serializer.Deserialize(packet.RecBuffer, 0, packet.DataSize) as NetPod;
        if (pod != null)
        {
            if (pod is NetAckPod)
            {
                // If this channel is set up to ack
                if (Channels[packet.ChannelType].ShouldAck)
                {
                    var ackPod = pod as NetAckPod;
                    var transmissionInfo = OutboundPods.FirstOrDefault(t => t.Pod.Index == ackPod.Index);
                    if (transmissionInfo != null)
                    {
                        // Record as acknowledged
                        triggerAckReceivedEvent(transmissionInfo.ConnectionId, transmissionInfo.Pod.Content, transmissionInfo.ChannelType);
                    }
                    else
                    {
                        Log.Debug(this, "Ack came too late and is no longer in the buffer, index: " + ackPod.Index);
                    }
                }
            }
            else if (pod is NetAtomPod)
            {
                if (Channels[packet.ChannelType].ShouldReceive)
                {
                    var atomPod = pod as NetAtomPod;
                    if (atomPod.Content != null)
                    {
                        triggerDataReceivedEvent(atomPod.Content, packet.ChannelType);
                        ReceivedAtoms.Add(atomPod.Content);

                        if(Channels[packet.ChannelType].ShouldAck)
                        {
                            // Ack that we received the packet back to the sender.
                            // TODO: Do we need to do connection checks here?
                            sendAck(packet.ConnectionId, packet.ChannelType, atomPod.Index);
                        }
                    }
                    else
                    {
                        Log.Error(this, "Null atom in pod.");
                    }
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

    private void triggerDataReceivedEvent(NetAtom atom, NetQosType channelType)
    {
        if (Channels.ContainsKey(channelType))
        {
            var channel = Channels[channelType];
            channel.AtomReceivedEvent.Invoke(atom);
        }
        else
        {
            Log.Debug(this, "Received a message from a channel that wasn't registered: {0}", channelType);
        }
    }

    private void Start()
    {

            // Register all the channels we will open.
            registerChannels(Channels, ReceiveChannelTypes, AckChannelTypes);
    }

    private void OnDestroy()
    {
            // Clear channels.
            Channels.Clear();
    }



    private void triggerAckReceivedEvent(int connectionId, NetAtom atom, NetQosType channelType)
    {
        if (Channels.ContainsKey(channelType))
        {
            var channel = Channels[channelType];
            channel.AtomAckEvent.Invoke(connectionId, atom);
        }
        else
        {
            Log.Debug(this, "Received a message from a channel that wasn't registered: {0}", channelType);
        }
    }
 */


/*



public class Client
{
    public NetConnection Connection;
    public List<NetAtom> ReceivedAtoms;
    public List<int> ReceivedAcks;

    // list of replicants to write the requests to.
    // list of replicants to update the client deltas.

    public void OnReceiveAtom(NetAtom atom)
    {
        // Honor the client's request
    }

    public void OnReceiveAck(int index)
    {
        // Update the replicants' state to reflect that this client is at date x.
        // Shouldn't a host have a delta for each client, rather than a global one?
    }
}

public class Server
{
    public NetConnection Connection;
    public List<NetAtom> ReceivedAtoms;
    public List<int> ReceivedAcks;
    
    // List of pending requests
    // List of ghost replicants to write the atoms to.

    public void OnReceiveAtom(NetAtom atom)
    {
        // Apply this atom to the client.
    }

    public void OnReceiveAck(int index)
    {
        // Update the client's requests to reflect that the request was received.
    }
}

using System.Collections.Generic;
using UnityEngine;

public class NetReplicationManager : MonoBehaviour 
{
    private List<NetBehaviourBase> _replicants = new List<NetBehaviourBase>();
    private int _latestId = 0;
    public float TickRate = 1 / 2f;

    // For testing, sample and apply everything in the game.
    private List<NetReplicationDelta> _snapshot = new List<NetReplicationDelta>();
    private float _lastTickTime = 0f;

    public bool IsHost;

    public void Spawn(GameObject go)
    {
        var components = go.GetInterfaces<NetBehaviourBase>();
        foreach(var component in components)
        {
            assignId(component);
            _replicants.Add(component);
        }

        // TODO: spawn this to on all clients.
    }

    public void Despawn(GameObject go)
    {
        var components = go.GetInterfaces<NetBehaviourBase>();
        foreach(var component in components)
        {
            _replicants.Remove(component);
        }
    }

    private void assignId(NetBehaviourBase component)
    {
        component.Init(_latestId++);
    }

    public void TakeSnapshot()
    {
        _snapshot.Clear();
        foreach(var component in _replicants)
        {
            var content = component.Sample();
            var sample = new NetReplicationDelta(component.Id, content);
            _snapshot.Add(sample);
        }
    }

    public void ApplySnapshot()
    {
        foreach(var sample in _snapshot)
        {
            var component = _replicants.Find(f => f.Id == sample.ReplicantId);
            if(component != null)
            {
                component.Apply(sample.Content);
            }
        }
    }

    private void Start()
    {
        //SL.Get<NetConnectionManager>().ReliableDataReceivedEvent.AddListener(onAtomReceived);
        //SL.Get<NetConnectionManager>().AddListener(onAtomReceived);
        //SL.Get<NetConnectionManager>().AddListener(onAtomAcknowledged);
    }

    private void OnDestroy()
    {
        if(SL.Exists && SL.Get<NetConnectionManager>())
        {
            //SL.Get<NetConnectionManager>().ReliableDataReceivedEvent.RemoveListener(onAtomReceived);
            //SL.Get<NetConnectionManager>().UnreliableDataReceivedEvent.RemoveListener(onAtomReceived);
            //SL.Get<NetConnectionManager>().AtomAcknowledgedEvent.RemoveListener(onAtomAcknowledged);
        }
    }

    private void Update()
    {
        if(_lastTickTime + TickRate < Time.time)
        {
            Tick();
            _lastTickTime = Time.time;
        }
    }

    private void Tick()
    {
        if(IsHost)
        {
            // Decide which replicants need to be sent.
            // Send them.

            // TEMP: Send everything every tick.
            foreach (var component in _replicants)
            {
                var content = component.Sample();
                var sample = new NetReplicationDelta(component.Id, content);

                foreach(var connection in SL.Get<NetConnectionManager>().Connections)
                {
                    SL.Get<NetConnectionManager>().Send(connection, NetQosType.Unreliable, sample);
                }
            }
        }
        else
        {
            // Anything?
        }
    }

    private void onAtomReceived(NetAtom atom)
    {
        // Right now we only care about server deltas.
        var delta = atom as NetReplicationDelta;
        if(delta != null)
        {
            if(IsHost)
            {
                // Process client requests.
            }
            else
            {
                // Apply host atoms to my replicants.
                // Discard any outdated atoms.

                var component = _replicants.Find(f => f.Id == delta.ReplicantId);
                if(component != null)
                {
                    component.Apply(delta.Content);
                }
            }
        }
    }

    private void onAtomAcknowledged(int connectionId, NetAtom atom)
    {
        if(IsHost)
        {
            // Update each client records for the replicant
            // Ignore acks that are outdated for each client.
        }
        else
        {
            // Update client request list.
        }
    }
}



    private void sendAck(int connectionId, NetQosType channelType, int index)
    {
        var pod = new NetAckPod()
        {
            Index = index
        };

        byte[] buffer = new byte[MaxAtomSize];
        int length = _serializer.Serialize(pod, buffer);
        SL.Get<NetTransportManager>().Send(connectionId, buffer, length, channelType);
    }

 */