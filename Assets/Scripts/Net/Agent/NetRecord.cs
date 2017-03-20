using System;

[Serializable]
public class NetRecord
{
    public float Timestamp;
    public NetAtom Atom;
    public NetQosType ChannelType;
    public NetAgent Agent;
}