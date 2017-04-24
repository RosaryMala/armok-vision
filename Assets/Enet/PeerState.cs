namespace ENet
{
    /// <summary>
    /// Enumerates the possible peer states.
    /// </summary>
    public enum PeerState
    {
        /// <summary>
        /// The peer is uninitialized.
        /// </summary>
        Uninitialized = -1,

        /// <summary>
        /// The peer is disconnected.
        /// </summary>
        Disconnected = 0,

        /// <summary>
        /// Connecting to the peer.
        /// </summary>
        Connecting = 1,

        /// <summary>
        /// Connection is being acknowledged.
        /// </summary>
        AcknowledgingConnect = 2,

        /// <summary>
        /// A connection is pending.
        /// </summary>
        ConnectionPending = 3,

        /// <summary>
        /// A connection has been established.
        /// </summary>
        ConnectionSucceeded = 4,

        /// <summary>
        /// The peer is connected.
        /// </summary>
        Connected = 5,

        /// <summary>
        /// The peer will be disconnected once all packets are sent.
        /// </summary>
        DisconnectingLater = 6,

        /// <summary>
        /// The peer is disconnecting.
        /// </summary>
        Disconnecting = 7,

        /// <summary>
        /// Disconnection is being acknowledged.
        /// </summary>
        AcknowledgingDisconnect = 8,

        /// <summary>
        /// The peer is a zombie.
        /// </summary>
        Zombie = 9
    }
}
