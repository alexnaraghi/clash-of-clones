
public struct PendingConnection
{
    public readonly string Ip;
    public readonly int ConnectionId;

    public PendingConnection(string ip, int connectionId)
    {
        Ip = ip;
        ConnectionId = connectionId;
    }
}