using System;

[Serializable]
public class NetAgent
{
    public readonly bool IsHost;
    public readonly bool IsLocal;
    public readonly NetConnection Connection;

    public NetAgent(NetConnection connection, bool isHost, bool isLocal)
    {
        IsHost = isHost;
        IsLocal = isLocal;
        Connection = connection;
    }

    public string GetId()
    {
        string id;
        if(Connection != null)
        {
            id = Connection.ConnectionId.ToString();
        }
        else
        {
            id = "Local Player";
        }
        return id;
    }
}