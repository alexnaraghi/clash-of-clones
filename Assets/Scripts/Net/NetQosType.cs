// Supported channels
// Mirrors the UNET QosType, but allows our higher level constructs to be de-coupled from that
// implementation of the transport layer.

//
// Summary:
//     ///
//     Enumeration of all supported quality of service channel modes.
//     ///
public enum NetQosType
{
    //
    // Summary:
    //     ///
    //     There is no guarantee of delivery or ordering.
    //     ///
    Unreliable = 0,
    //
    // Summary:
    //     ///
    //     There is no guarantee of delivery or ordering, but allowing fragmented messages
    //     with up to 32 fragments per message.
    //     ///
    UnreliableFragmented = 1,
    //
    // Summary:
    //     ///
    //     There is no guarantee of delivery and all unordered messages will be dropped.
    //     Example: VoIP.
    //     ///
    UnreliableSequenced = 2,
    //
    // Summary:
    //     ///
    //     Each message is guaranteed to be delivered but not guaranteed to be in order.
    //     ///
    Reliable = 3,
    //
    // Summary:
    //     ///
    //     Each message is guaranteed to be delivered, also allowing fragmented messages
    //     with up to 32 fragments per message.
    //     ///
    ReliableFragmented = 4,
    //
    // Summary:
    //     ///
    //     Each message is guaranteed to be delivered and in order.
    //     ///
    ReliableSequenced = 5,
    //
    // Summary:
    //     ///
    //     An unreliable message. Only the last message in the send buffer is sent. Only
    //     the most recent message in the receive buffer will be delivered.
    //     ///
    StateUpdate = 6,
    //
    // Summary:
    //     ///
    //     A reliable message. Note: Only the last message in the send buffer is sent. Only
    //     the most recent message in the receive buffer will be delivered.
    //     ///
    ReliableStateUpdate = 7,
    //
    // Summary:
    //     ///
    //     A reliable message that will be re-sent with a high frequency until it is acknowledged.
    //     ///
    AllCostDelivery = 8
}