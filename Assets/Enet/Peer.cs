#region License
/*
ENet for C#
Copyright (c) 2011-2013 James F. Bellinger <jfb@zer7.com>

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
    /// Represents a connection with another computer.
    /// </summary>
    public unsafe struct Peer : IEquatable<Peer>
    {
        ENetPeer* _peer;

        /// <summary>
        /// Initializes a peer based on a native C peer.
        /// </summary>
        /// <param name="peer">The native C peer.</param>
        public Peer(ENetPeer* peer)
        {
            this = new Peer() { NativeData = peer };
        }

        void CheckInitialized()
        {
            if (NativeData == null) { throw new InvalidOperationException("Not initialized."); }
        }

        public override bool Equals(object obj)
        {
            return obj is Peer && Equals((Peer)obj);
        }

        public bool Equals(Peer other)
        {
            return NativeData == other.NativeData;
        }

        public override int GetHashCode()
        {
            return (int)(long)NativeData; // ENet types are malloc'ed. They do not move.
        }

        /// <summary>
        /// Configures throttling. ENet measures lag over an interval, and alters its throttle parameter
        /// based on individual packet round-trip times versus the mean. This parameter controls the probability
        /// ENet will drop an unreliable packet. If a packet has a smaller round-trip time than average, the parameter
        /// is increased by the acceleration term, causing less packets to be dropped. If a packet has a larger
        /// round-trip time than average, the parameter is decreased by the deceleration term, causing more packets
        /// to be dropped.
        /// </summary>
        /// <param name="interval">The interval in milliseconds over which to measure. The default is 5000.</param>
        /// <param name="acceleration">Acceleration rate. The default value is 2, and the limit is 32.</param>
        /// <param name="deceleration">Deceleration rate. The default value is 2, and the limit is 32.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="interval"/> is negative,
        ///     <paramref name="acceleration"/> is less than 0 or greater than 32, and/or
        ///     <paramref name="deceleration"/> is less than 0 or greater than 32.
        /// </exception>
        /// <exception cref="InvalidOperationException">The peer is not initialized.</exception>
        public void ConfigureThrottle(int interval, int acceleration, int deceleration)
        {
            CheckInitialized();
            if (interval < 0) { throw new ArgumentOutOfRangeException("interval"); }
            if (acceleration < 0 || acceleration > 32) { throw new ArgumentOutOfRangeException("acceleration"); }
            if (deceleration < 0 || deceleration > 32) { throw new ArgumentOutOfRangeException("deceleration"); }
            ENetApi.enet_peer_throttle_configure(NativeData, (uint)interval, (uint)acceleration, (uint)deceleration);
        }

        /// <summary>
        /// Gracefully disconnects from the remote computer.
        /// A disconnect event occurs.
        /// </summary>
        /// <param name="data">Data to send along with the disconnect packet.</param>
        /// <exception cref="InvalidOperationException">The peer is not initialized.</exception>
        public void Disconnect(int data)
        {
            CheckInitialized(); ENetApi.enet_peer_disconnect(NativeData, (uint)data);
        }

        /// <summary>
        /// Gracefully disconnects from the remote computer after all outgoing data has been sent.
        /// A disconnect event occurs.
        /// </summary>
        /// <param name="data">Data to send along with the disconnect packet.</param>
        /// <exception cref="InvalidOperationException">The peer is not initialized.</exception>
        public void DisconnectLater(int data)
        {
            CheckInitialized(); ENetApi.enet_peer_disconnect_later(NativeData, (uint)data);
        }

        /// <summary>
        /// Immediately disconnects from the remote computer.
        /// A disconnect packet is sent unreliably. No event occurs.
        /// </summary>
        /// <param name="data">Data to send along with the disconnect packet.</param>
        /// <exception cref="InvalidOperationException">The peer is not initialized.</exception>
        public void DisconnectNow(int data)
        {
            CheckInitialized(); ENetApi.enet_peer_disconnect_now(NativeData, (uint)data);
        }

        /// <summary>
        /// Sends a ping to the remote computer.
        /// </summary>
        /// <exception cref="InvalidOperationException">The peer is not initialized.</exception>
        public void Ping()
        {
            CheckInitialized(); ENetApi.enet_peer_ping(NativeData);
        }

        /// <summary>
        /// Resets the connection to the remote computer.
        /// No disconnect packets are sent, and no event occurs.
        /// </summary>
        /// <exception cref="InvalidOperationException">The peer is not initialized.</exception>
        public void Reset()
        {
            CheckInitialized(); ENetApi.enet_peer_reset(NativeData);
        }

        /// <summary>
        /// Gets the remote address.
        /// </summary>
        /// <exception cref="InvalidOperationException">The peer is not initialized.</exception>
        public IPEndPoint GetRemoteAddress()
        {
            CheckInitialized(); return NativeData->address;
        }

        /// <summary>
        /// Dequeue a received packet.
        /// </summary>
        /// <param name="channelID">The ID of the channel the packet was sent on.</param>
        /// <param name="packet">The received packet.</param>
        /// <returns>True if a packet was dequeued, or false if there are no more packets.</returns>
        /// <exception cref="InvalidOperationException">The peer is not initialized.</exception>
        public bool Receive(out byte channelID, out Packet packet)
        {
            CheckInitialized(); ENetPacket* nativePacket;
            nativePacket = ENetApi.enet_peer_receive(NativeData, out channelID);
            if (nativePacket == null) { packet = new Packet(); return false; }
            packet = new Packet(nativePacket); return true;
        }

        /// <summary>
        /// Enqueues a packet for sending using the given data and flags.
        /// </summary>
        /// <param name="channelID">The ID of the channel to send on.</param>
        /// <param name="data">The data to send.</param>
        /// <param name="flags">The packet flags.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The peer is not initialized.</exception>
        /// <exception cref="ENetException">An error occured.</exception>
        public void Send(byte channelID, byte[] data, PacketFlags flags)
        {
            if (data == null) { throw new ArgumentNullException("data"); }
            Send(channelID, data, 0, data.Length, flags);
        }

        /// <summary>
        /// Enqueues a packet for sending using the given data, offsets, length, and flags.
        /// </summary>
        /// <param name="channelID">The ID of the channel to send on.</param>
        /// <param name="data">The array containing the data to send.</param>
        /// <param name="offset">The index of the first byte of data.</param>
        /// <param name="length">The length of the data.</param>
        /// <param name="flags">The packet flags.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> and/or <paramref name="length"/> are out of range.</exception>
        /// <exception cref="InvalidOperationException">The peer is not initialized.</exception>
        /// <exception cref="ENetException">An error occured.</exception>
        public void Send(byte channelID, byte[] data, int offset, int length, PacketFlags flags)
        {
            if (data == null) { throw new ArgumentNullException("data"); }
            using (Packet packet = new Packet())
            {
                packet.Initialize(data, offset, length, flags);
                Send(channelID, packet);
            }
        }

        /// <summary>
        /// Enqueues a packet for sending.
        /// </summary>
        /// <param name="channelID">The ID of the channel to send on.</param>
        /// <param name="packet">The packet to send.</param>
        /// <returns>True if the packet was enqueued successfully, or false if an error occured.</returns>
        /// <exception cref="InvalidOperationException">The peer is not initialized.</exception>
        /// <exception cref="ENetException">An error occured.</exception>
        public void Send(byte channelID, Packet packet)
        {
            CheckInitialized(); packet.CheckInitialized();
            int ret = ENetApi.enet_peer_send(NativeData, channelID, packet.NativeData);
            if (ret < 0) { throw new ENetException("An error occured sending to the peer."); }
        }

        /// <summary>
        /// Sets the interval between pings.
        /// ENet will automatically send pings when it hasn't received anything from the remote computer.
        /// </summary>
        /// <param name="interval">
        ///     The interval in milliseconds between pings, or 0 to use the default (500).
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is negative.</exception>
        /// <exception cref="InvalidOperationException">The peer is not initialized.</exception>
        /// <exception cref="NotSupportedException">This method requires ENet 1.3.4 or newer.</exception>
        public void SetPingInterval(int interval)
        {
            CheckInitialized();
            if (interval < 0) { throw new ArgumentOutOfRangeException("interval"); }

            try
            {
                ENetApi.enet_peer_ping_interval(NativeData, (uint)interval);
            }
            catch (EntryPointNotFoundException e)
            {
                throw new NotSupportedException("This method requires ENet 1.3.4 or newer.", e);
            }
        }

        /// <summary>
        /// Sets timeouts for a response from the remote computer acknowledging its receipt of a reliable packet.
        /// When the timeouts are exceeded, a disconnect event occurs.
        /// </summary>
        /// <param name="retryLimit">
        ///     The number of retries to make before considering the minimum timeout time.
        ///     The default value is 5.
        /// </param>
        /// <param name="retryMinimumTime">
        ///     The minimum time in milliseconds to allow for retrying, or 0 to use the default (5000).
        /// </param>
        /// <param name="maximumTime">
        ///     The maximum time in milliseconds to wait regardless of the number of retries, or 0 to use the default (30000).
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="retryLimit"/> is negative or greater than 20,
        ///     <paramref name="retryMinimumTime"/> is negative, and/or
        ///     <paramref name="maximumTime"/> is negative.
        ///  </exception>
        /// <exception cref="InvalidOperationException">The peer is not initialized.</exception>
        /// <exception cref="NotSupportedException">This method requires ENet 1.3.4 or newer.</exception>
        public void SetTimeouts(int retryLimit, int retryMinimumTime, int maximumTime)
        {
            CheckInitialized();
            if (retryLimit < 0 || retryLimit > 20) { throw new ArgumentOutOfRangeException("retryLimit"); }
            if (retryMinimumTime < 0) { throw new ArgumentOutOfRangeException("retryMinimumTime"); }
            if (maximumTime < 0) { throw new ArgumentOutOfRangeException("maximumTime"); }

            try
            {
                ENetApi.enet_peer_timeout(NativeData, 1U << retryLimit, (uint)retryMinimumTime, (uint)maximumTime);
            }
            catch (EntryPointNotFoundException e)
            {
                throw new NotSupportedException("This method requires ENet 1.3.4 or newer.", e);
            }
        }

        /// <summary>
        /// Gets the host associated with this peer.
        /// </summary>
        public Host Host
        {
            get { CheckInitialized(); return new Host(NativeData->host); }
        }

        /// <summary>
        /// Gets or sets the native C peer.
        /// </summary>
        public ENetPeer* NativeData
        {
            get { return _peer; }
            set { _peer = value; }
        }

        /// <summary>
        /// Gets the peer state.
        /// </summary>
        public PeerState State
        {
            get { return IsInitialized ? NativeData->state : PeerState.Uninitialized; }
        }

        /// <summary>
        /// Gets or sets the user data associated with this peer.
        /// </summary>
        public IntPtr UserData
        {
            get { CheckInitialized(); return NativeData->data; }
            set { CheckInitialized(); NativeData->data = value; }
        }

        /// <summary>
        /// Returns true if the peer is initialized.
        /// </summary>
        public bool IsInitialized
        {
            get { return NativeData != null; }
        }

        public static bool operator ==(Peer peer1, Peer peer2)
        {
            return peer1.Equals(peer2);
        }

        public static bool operator !=(Peer peer1, Peer peer2)
        {
            return !peer1.Equals(peer2);
        }
    }
}
