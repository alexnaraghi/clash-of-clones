using System;
using UnityEngine.Networking;

[Serializable]
public class RawPacket
{
    public const int BUFFER_SIZE = 1024;
    public int RecHostId; 
    public int ConnectionId; 
    public int ChannelId; 
    public byte[] RecBuffer = new byte[BUFFER_SIZE];
    public int DataSize;
    public byte ResponseCode;

    public override string ToString()
    {
        return string.Format(
            @"response:  {0}
              host:      {1}
              connectId: {2}
              channel:   {3}
              dataSize:  {4}",
              ((NetworkError)ResponseCode).ToString(),
              RecHostId,
              ConnectionId,
              ChannelId,
              DataSize);
    }
}