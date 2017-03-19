using System;

/*
using ChannelStreamDictionary 
    = System.Collections.Generic.Dictionary<UnityEngine.Networking.QosType, NetChannelStream>;

[Serializable]
public class Connection
{
    public readonly ChannelStreamDictionary Streams;
    public readonly string Ip;
    public readonly int ConnectionId;

    public Connection(string ip, int connectionId, params QosType[] channelTypes)
    {
        Streams = new ChannelStreamDictionary();
        foreach (var channel in channelTypes)
        {
            Streams.Add(channel, new NetChannelStream());
        }
        Ip = ip;
        ConnectionId = connectionId;
    }
} */