using System;

[Serializable]
public class NetConnection
{
    public readonly string Ip;
    public readonly int ConnectionId;

    public NetConnection(string ip, int connectionId)
    {
        Ip = ip;
        ConnectionId = connectionId;
    }
}