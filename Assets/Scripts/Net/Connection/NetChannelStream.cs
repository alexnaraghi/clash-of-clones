using System;

/*
[Serializable]
public class NetChannelStream
{
    public const int PACKET_BUFFER_SIZE = 64;
    public const int ATOM_BUFFER_SIZE = 64;
    
    public readonly CircularBuffer<PacketRequest> _outboundPackets;
    public readonly CircularBuffer<NetAtom> _pendingAtoms;

    private int _nextPacketIndex;

    public NetChannelStream()
    {
        _outboundPackets = new CircularBuffer<PacketRequest>(PACKET_BUFFER_SIZE);
        _pendingAtoms = new CircularBuffer<PacketRequest>(ATOM_BUFFER_SIZE);
    }

    public int ClaimNextPacketIndex()
    {
        return _nextPacketIndex++;
    }
}

 */