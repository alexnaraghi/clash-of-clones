using System;

[Serializable]
public class NetPodTransmissionInfo
{
    public NetAtomPod Pod;
    public int ConnectionId;
    public NetQosType ChannelType;

    public NetPodTransmissionInfo(int connectionId, NetQosType channelType, NetAtomPod pod)
    {
        ConnectionId = connectionId;
        ChannelType = channelType;
        Pod = pod;
    }
}