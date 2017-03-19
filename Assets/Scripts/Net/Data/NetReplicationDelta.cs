using System;

[Serializable]
public class NetReplicationDelta : NetAtom
{
    public int ReplicantId;
    public object Content;

    public NetReplicationDelta(int id, object content)
    {
        ReplicantId = id;
        Content = content;
    }
}