using System;

[Serializable]
public class NetTransportData
{
    public int RecHostId; 
    public int ConnectionId; 
    public int ChannelId;
    public NetQosType ChannelType;
    public byte[] RecBuffer = new byte[NetTransportManager.MAX_PACKET_SIZE];
    public int DataSize;
    public byte ResponseCode;

    public override string ToString()
    {
        return string.Format(
            @"response: {0}
              host: {1}
              connectId: {2}
              channelType: {3}
              channelId: {4}
              dataSize: {5}",
              ((NetError)ResponseCode).ToString(),
              RecHostId,
              ConnectionId,
              ChannelType,
              ChannelId,
              DataSize);
    }
}