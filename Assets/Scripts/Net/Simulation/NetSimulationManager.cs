using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NetSimulationManager : MonoBehaviour
{
    public NetReplicationStream Replication;
    public NetChatStream Chat;

    public bool ClientsReady { get; }

    public void StartMatch()
    {

    }

    private void Start()
    {
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
}
