#region License
/*
ENet for C#
Copyright (c) 2011-2012 James F. Bellinger <jfb@zer7.com>

Permission to use, copy, modify, and/or distribute this software for any
purpose with or without fee is hereby granted, provided that the above
copyright notice and this permission notice appear in all copies.

THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*/
#endregion

using System;
using System.Net;
using System.Net.Sockets;
using ENet.Native;

namespace ENet
{
    /// <summary>
    /// Acts as either client or peer-to-peer/server.
    /// </summary>
    public unsafe struct Host : IDisposable, IEquatable<Host>
    {
        ENetHost* _host;

        /// <summary>
        /// Initializes a host based on a native C host.
        /// </summary>
        /// <param name="peer">The native C peer.</param>
        public Host(ENetHost* host)
        {
            this = new Host() { NativeData = host };
        }

        public override bool Equals(object obj)
        {
            return obj is Host && Equals((Host)obj);
        }

        public bool Equals(Host other)
        {
            return NativeData == other.NativeData;
        }

        public override int GetHashCode()
        {
            return (int)(long)NativeData; // ENet types are malloc'ed. They do not move.
        }

        static void CheckBandwidthLimit(int incomingBandwidth, int outgoingBandwidth)
        {
            if (incomingBandwidth < 0) { throw new ArgumentOutOfRangeException("incomingBandwidth"); }
            if (outgoingBandwidth < 0) { throw new ArgumentOutOfRangeException("outgoingBandwidth"); }
        }

        static void CheckChannelLimit(int channelLimit)
        {
            if (channelLimit < 0 || channelLimit > ENetApi.ENET_PROTOCOL_MAXIMUM_CHANNEL_COUNT)
                { throw new ArgumentOutOfRangeException("channelLimit"); }
        }

        void CheckInitialized()
        {
            if (NativeData == null) { throw new InvalidOperationException("Not initialized."); }
        }

        /// <summary>
        /// Initialize a host that will not accept connections.
        /// </summary>
        /// <param name="peerLimit">
        ///     The maximum number of peers for this host.
        ///     If you are only connecting to one server, set this to 1.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="peerLimit"/> is less than 0 or greater than 4095.
        /// </exception>
        /// <exception cref="InvalidOperationException">The host is already initialized.</exception>
        /// <exception cref="ENetException">Failed to initialize the host.</exception>
        public void InitializeClient(int peerLimit)
        {
            Initialize(null, peerLimit);
        }

        /// <summary>
        /// Initialize a host that will accept connections on any IP address.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        /// <param name="peerLimit">The maximum number of peers for this host.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="port"/> is less than 0 or greater than 65535, and/or
        ///     <paramref name="peerLimit"/> is less than 0 or greater than 4095.
        /// </exception>
        /// <exception cref="InvalidOperationException">The host is already initialized.</exception>
        /// <exception cref="ENetException">Failed to initialize the host.</exception>
        public void InitializeServer(int port, int peerLimit)
        {
            Initialize(new IPEndPoint(IPAddress.Any, port), peerLimit);
        }

        /// <summary>
        /// Initialize a host that will accept connections on a particular address, or not accept connections.
        /// </summary>
        /// <param name="address">The address to listen on, or null to not accept connections.</param>
        /// <param name="peerLimit">The maximum number of peers for this host.</param>
        /// <param name="channelLimit">The maximum number of channels, or 0 to use the maximum possible (255).</param>
        /// <param name="incomingBandwidth">The maximum incoming rate of transfer, or 0 for no limit.</param>
        /// <param name="outgoingBandwidth">The maximum outgoing rate of transfer, or 0 for no limit.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="peerLimit"/> is less than 0 or greater than 4095,
        ///     <paramref name="channelLimit"/> is less than 0 or greater than 255,
        ///     <paramref name="incomingBandwidth"/> is less than 0, and/or
        ///     <paramref name="outgoingBandwidth"/> is less than 0.
        /// </exception>
        /// <exception cref="InvalidOperationException">The host is already initialized.</exception>
        /// <exception cref="ENetException">Failed to initialize the host.</exception>
        public void Initialize(IPEndPoint address, int peerLimit,
                               int channelLimit = 0, int incomingBandwidth = 0, int outgoingBandwidth = 0)
        {
            if (NativeData != null) { throw new InvalidOperationException("Already initialized."); }
            if (peerLimit < 0 || peerLimit > ENetApi.ENET_PROTOCOL_MAXIMUM_PEER_ID)
                { throw new ArgumentOutOfRangeException("peerLimit"); }
            CheckChannelLimit(channelLimit);
            CheckBandwidthLimit(incomingBandwidth, outgoingBandwidth);

            if (address != null)
            {
                ENetAddress nativeAddress = (ENetAddress)address;
                NativeData = ENetApi.enet_host_create(ref nativeAddress, (IntPtr)peerLimit,
                    (IntPtr)channelLimit, (uint)incomingBandwidth, (uint)outgoingBandwidth);
            }
            else
            {
                NativeData = ENetApi.enet_host_create(null, (IntPtr)peerLimit,
                    (IntPtr)channelLimit, (uint)incomingBandwidth, (uint)outgoingBandwidth);
            }
            if (NativeData == null) { throw new ENetException("Host creation call failed."); }
        }

        /// <summary>
        /// Destroys the host.
        /// </summary>
        public void Dispose()
        {
            if (NativeData != null)
            {
                ENetApi.enet_host_destroy(NativeData);
                NativeData = null;
            }
        }

        /// <summary>
        /// Broadcast a packet to all peers.
        /// </summary>
        /// <param name="channelID">The ID of the channel</param>
        /// <param name="packet">The packet to send.</param>
        /// <remarks>ENet takes ownership of the packet. Do not call methods on it afterwards.</remarks>
        /// <exception cref="InvalidOperationException">The host is not initialized.</exception>
        public void Broadcast(byte channelID, ref Packet packet)
        {
            CheckInitialized(); packet.CheckInitialized();
            
            bool clear = packet.ReferenceCount == 0;
            ENetApi.enet_host_broadcast(NativeData, channelID, packet.NativeData);
            if (clear) { packet.NativeData = null; } // Broadcast may automatically free in this case.
        }

        /// <summary>
        /// Enables compression using the range encoder.
        /// </summary>
        /// <exception cref="InvalidOperationException">The host is not initialized.</exception>
        /// <exception cref="ENetException">Failed to create range encoder. This is likely due to low memory.</exception>
        public void CompressWithRangeEncoder()
        {
            CheckInitialized();
            int ret = ENetApi.enet_host_compress_with_range_encoder(NativeData);
            if (ret < 0) { throw new ENetException("Failed to create range encoder."); }
        }

        /// <summary>
        /// Disables compression.
        /// </summary>
        /// <exception cref="InvalidOperationException">The host is not initialized.</exception>
        public void DoNotCompress()
        {
            CheckInitialized();
            ENetApi.enet_host_compress(NativeData, null);
        }

        /// <summary>
        /// Checks for queued events.
        /// </summary>
        /// <param name="event">The dequeued event.</param>
        /// <returns>True if an event was dequeued, otherwise false.</returns>
        /// <exception cref="InvalidOperationException">The host is not initialized.</exception>
        /// <exception cref="ENetException">An error occured while checking events.</exception>
        public bool CheckEvents(out Event @event)
        {
            CheckInitialized(); ENetEvent nativeEvent;
            int ret = ENetApi.enet_host_check_events(NativeData, out nativeEvent);
            if (ret < 0) { throw new ENetException("Error while checking for events."); }
            if (ret == 0) { @event = new Event(); return false; }
            @event = new Event(nativeEvent); return true;
        }

        /// <summary>
        /// Connects to a remote computer at the given host and port.
        /// </summary>
        /// <param name="hostName">The IP address or host name to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="data">Data to send along with the connect packet.</param>
        /// <param name="channelLimit">The maximum number of channels, or 0 to use the maximum possible (255).</param>
        /// <returns>
        ///     The new peer. This method does not block: the connection will be established
        ///     when you receive a <see cref="EventType.Connect"/> event.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="hostName"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="hostName"/> is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="hostName"/> is too long,
        ///     <paramref name="port"/> is less than 0 or greater than 65535, and/or
        ///     <paramref name="channelLimit"/> is less than 0 or greater than 255.
        /// </exception>
        /// <exception cref="InvalidOperationException">The host is not initialized.</exception>
        /// <exception cref="SocketException">Host name lookup failed, or no IPv4 hosts were available.</exception>
        /// <exception cref="ENetException">An error occured.</exception>
        public Peer Connect(string hostName, int port, int data, int channelLimit = 0)
        {
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);
            foreach (IPAddress address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return Connect(new IPEndPoint(address, port), data, channelLimit);
                }
            }
            throw new SocketException();
        }

        /// <summary>
        /// Connects to a remote computer at the given address.
        /// </summary>
        /// <param name="address">The address to connect to.</param>
        /// <param name="data">Data to send along with the connect packet.</param>
        /// <param name="channelLimit">The maximum number of channels, or 0 to use the maximum possible (255).</param>
        /// <returns>
        ///     The new peer. This method does not block: the connection will be established
        ///     when you receive a <see cref="EventType.Connect"/> event.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="address"/> is not IPv4.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="address"/>'s port is less than 0 or greater than 65535, and/or
        ///     <paramref name="channelLimit"/> is less than 0 or greater than 255.
        /// </exception>
        /// <exception cref="InvalidOperationException">The host is not initialized.</exception>
        /// <exception cref="ENetException">An error occured.</exception>
        public Peer Connect(IPEndPoint address, int data, int channelLimit = 0)
        {
            CheckInitialized(); ENetAddress nativeAddress = (ENetAddress)address;
            CheckChannelLimit(channelLimit);

            // For consistency with Connect() and SetChannelLimit(),
            if (channelLimit == 0) { channelLimit = (int)ENetApi.ENET_PROTOCOL_MAXIMUM_CHANNEL_COUNT; }

            Peer peer = new Peer(ENetApi.enet_host_connect(NativeData, ref nativeAddress, (IntPtr)channelLimit, (uint)data));
            if (peer.NativeData == null) { throw new ENetException("Host connect failed."); }
            return peer;
        }

        /// <summary>
        /// Sends queued packets immediately. Normally they are sent when you call <see cref="Service(int, out Event)"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The host is not initialized.</exception>
        public void Flush()
        {
            CheckInitialized();
            ENetApi.enet_host_flush(NativeData);
        }

        /// <summary>
        /// Sends queued outgoing packets, receives incoming packets, and handles connection events.
        /// </summary>
        /// <param name="timeout">Timeout in milliseconds to wait for an event. For polling, use 0.</param>
        /// <param name="event">The event.</param>
        /// <returns>True if an event occured, otherwise false.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
        /// <exception cref="InvalidOperationException">The host is not initialized.</exception>
        /// <exception cref="ENetException">An error occured.</exception>
        public bool Service(int timeout, out Event @event)
        {
            if (timeout < 0) { throw new ArgumentOutOfRangeException("timeout"); }
            CheckInitialized(); ENetEvent nativeEvent = new ENetEvent();

            // As of 1.3.6, ENet is not signal safe, and Mono uses signals for garbage collection.
            // 
            // So, there's really nothing better we can do than retry. Luckily the cases that return -1
            // are cases that return immediately or this could cause lockups.
            //
            // The entire situation is still very dicey with MonoDevelop.
            // A proper fix really has to be done in the ENet native library.
            // If you want to eliminate this entirely and don't care about these spurious
            // failures of enet_host_service, try/catch the ENetException and just ignore it.
            // That's essentially what I am doing here, except with an upper limit so real errors
            // can get through...
            int ret = -1;
            for (int nretries = 0; nretries < 1000 && ret == -1; nretries++)
            {
                ret = ENetApi.enet_host_service(NativeData, out nativeEvent, (uint)timeout);
            }

            if (ret < 0) { throw new ENetException(string.Format("Service failed (native data {0}).", (IntPtr)NativeData)); }
            if (ret == 0) { @event = new Event(); return false; }
            @event = new Event(nativeEvent); return true;
        }

        /// <summary>
        /// Set the bandwidth limit.
        /// </summary>
        /// <param name="incomingBandwidth">The maximum incoming rate of transfer, or 0 for no limit.</param>
        /// <param name="outgoingBandwidth">The maximum outgoing rate of transfer, or 0 for no limit.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="incomingBandwidth"/> is less than 0, and/or
        ///     <paramref name="outgoingBandwidth"/> is less than 0.
        /// </exception>
        /// <exception cref="InvalidOperationException">The host is not initialized.</exception>
        public void SetBandwidthLimit(int incomingBandwidth, int outgoingBandwidth)
        {
            CheckInitialized(); CheckBandwidthLimit(incomingBandwidth, outgoingBandwidth);
            ENetApi.enet_host_bandwidth_limit(NativeData, (uint)incomingBandwidth, (uint)outgoingBandwidth);
        }

        /// <summary>
        /// Set the channel limit.
        /// </summary>
        /// <param name="channelLimit">The maximum number of channels, or 0 to use the maximum possible (255).</param>
        /// <exception cref="InvalidOperationException">The host is not initialized.</exception>
        public void SetChannelLimit(int channelLimit)
        {
            CheckChannelLimit(channelLimit); CheckInitialized();
            ENetApi.enet_host_channel_limit(NativeData, (IntPtr)channelLimit);
        }

        /// <summary>
        /// Gets or sets the native C host.
        /// </summary>
        public ENetHost* NativeData
        {
            get { return _host; }
            set { _host = value; }
        }

        /// <summary>
        /// Returns true if the host is initialized.
        /// </summary>
        public bool IsInitialized
        {
            get { return NativeData != null; }
        }

        public static bool operator ==(Host host1, Host host2)
        {
            return host1.Equals(host2);
        }

        public static bool operator !=(Host host1, Host host2)
        {
            return !host1.Equals(host2);
        }
    }
}
