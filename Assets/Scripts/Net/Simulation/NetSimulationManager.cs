using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class NetSimulationManager : MonoBehaviour
{
    public NetReplicationStream Replication;
    public NetChatStream Chat;

    private List<INetStream> _streams = new List<INetStream>();

    public bool ClientsReady { get; }

    public void StartHost()
    {
        SL.Get<NetAgentManager>().StartHost(getRequiredChannels());
    }

    public void StartClient()
    {
        SL.Get<NetAgentManager>().StartClient(getRequiredChannels());
    }

    public void Disconnect()
    {
        SL.Get<NetAgentManager>().Disconnect();
    }

    public void StartMatch()
    {

    }

    private void Start()
    {
        Replication = GetComponent<NetReplicationStream>();
        Chat = GetComponent<NetChatStream>();

        _streams.Add(Replication);
        _streams.Add(Chat);

        // Test code
        Chat = GetComponent<NetChatStream>();
        Chat.EnableStream();
    }

    private void Update()
    {
        // If clients get disconnected, or the match is interrupted, pause the match stream.
    }
    

    private void onSimulationInterrupted()
    {

    }

    private void OnSimulationResumed()
    {

    }

    /// <summary>
    /// Merges the required channels of all the streams.
    /// </summary>
    private NetQosType[] getRequiredChannels()
    {
        return _streams.Where(s => s != null).SelectMany(s => s.RequiredChannels).Distinct().ToArray();
    }
}
