
// We may want to at some point put a prioritization layer in between the game-server and the streams.
public interface INetStream
{
    NetQosType[] RequiredChannels { get; }
    void EnableStream();
    void DisableStream();
}
