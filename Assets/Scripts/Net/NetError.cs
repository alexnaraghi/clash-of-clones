public enum NetError
{
    //
    // Summary:
    //     ///
    //     The operation completed successfully.
    //     ///
    Ok = 0,
    //
    // Summary:
    //     ///
    //     The specified host not available.
    //     ///
    WrongHost = 1,
    //
    // Summary:
    //     ///
    //     The specified connectionId doesn't exist.
    //     ///
    WrongConnection = 2,
    //
    // Summary:
    //     ///
    //     The specified channel doesn't exist.
    //     ///
    WrongChannel = 3,
    //
    // Summary:
    //     ///
    //     Not enough resources are available to process this request.
    //     ///
    NoResources = 4,
    //
    // Summary:
    //     ///
    //     Not a data message.
    //     ///
    BadMessage = 5,
    //
    // Summary:
    //     ///
    //     Connection timed out.
    //     ///
    Timeout = 6,
    //
    // Summary:
    //     ///
    //     The message is too long to fit the buffer.
    //     ///
    MessageToLong = 7,
    //
    // Summary:
    //     ///
    //     Operation is not supported.
    //     ///
    WrongOperation = 8,
    //
    // Summary:
    //     ///
    //     The protocol versions are not compatible. Check your library versions.
    //     ///
    VersionMismatch = 9,
    //
    // Summary:
    //     ///
    //     The Networking.ConnectionConfig does not match the other endpoint.
    //     ///
    CRCMismatch = 10,
    //
    // Summary:
    //     ///
    //     The address supplied to connect to was invalid or could not be resolved.
    //     ///
    DNSFailure = 11
}