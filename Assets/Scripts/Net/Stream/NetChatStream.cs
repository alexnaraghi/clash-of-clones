using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetChatStream : MonoBehaviour, INetStream
{
    /// <summary>
    /// Our messages we wish to send to all clients.
    /// </summary>
    public List<NetChatMessage> MessagesToSend = new List<NetChatMessage>();
    
    /// <summary>
    /// The chat log that has been validated by the server.
    /// </summary>
    public List<NetChatServerMessage> ChatLog = new List<NetChatServerMessage>();

    private bool _isEnabled;

    public void DisableStream()
    {
        _isEnabled = false;
    }

    public void EnableStream()
    {
        _isEnabled = true;
    }

    private void Start()
    {
        var network = SL.Get<NetAgentManager>();
        network.ClientDataReceivedEvent.AddListener(onClientDataReceived);
        network.ServerDataReceivedEvent.AddListener(onServerDataReceived);
    }

    private void Update()
    {
        if(_isEnabled)
        {
            var network = SL.Get<NetAgentManager>();
            foreach(var message in MessagesToSend)
            {
                network.ClientSend(message, NetQosType.Reliable);
            }
            MessagesToSend.Clear();
        }
    }

    private void onServerDataReceived(NetServerAtom atom, NetQosType channelType)
    {
        var message = atom as NetChatServerMessage;
        if (message != null)
        {
            ChatLog.Add(message);
        }
    }

    private void onClientDataReceived(NetClientAtom atom, NetAgent sender, NetQosType channelType)
    {
        var request = atom as NetChatMessage;
        if(sender != null && request != null)
        {
            var message = new NetChatServerMessage()
            {
                AuthorId = sender.GetId(),
                Content = request.Content
            };

            var network = SL.Get<NetAgentManager>();
            foreach(var client in network.Clients)
            {
                if(client.IsLocal)
                {
                    // Don't send data to the local player over the network.
                    onServerDataReceived(message, NetQosType.Reliable);
                }
                else
                {
                    network.HostSend(message, client, NetQosType.Reliable);
                }
            }
        }
    }
}

[Serializable]
public class NetChatMessage : NetClientAtom
{
    public string Content;
}

[Serializable]
public class NetChatServerMessage : NetServerAtom
{
    public string AuthorId;
    public string Content;
}