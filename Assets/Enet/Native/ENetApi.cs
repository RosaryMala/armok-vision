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

#pragma warning disable 1591

using System;
using System.Runtime.InteropServices;

namespace ENet.Native
{
    public unsafe sealed class ENetApi
    {
        public const uint ENET_HOST_ANY = 0;
        public const uint ENET_HOST_BROADCAST = 0xffffffff;
        public const uint ENET_PEER_PACKET_THROTTLE_SCALE = 32;
        public const uint ENET_PEER_PACKET_THROTTLE_ACCELERATION = 2;
        public const uint ENET_PEER_PACKET_THROTTLE_DECELERATION = 2;
        public const uint ENET_PEER_PACKET_THROTTLE_INTERVAL = 5000;
        public const uint ENET_PEER_PING_INTERVAL = 500;
        public const uint ENET_PEER_TIMEOUT_LIMIT = 32;
        public const uint ENET_PEER_TIMEOUT_MINIMUM = 5000;
        public const uint ENET_PEER_TIMEOUT_MAXIMUM = 30000;
        public const uint ENET_PROTOCOL_MINIMUM_CHANNEL_COUNT = 0x01;
        public const uint ENET_PROTOCOL_MAXIMUM_CHANNEL_COUNT = 0xff;
        public const uint ENET_PROTOCOL_MAXIMUM_FRAGMENT_COUNT = 1024 * 1024;
        public const uint ENET_PROTOCOL_MAXIMUM_PACKET_SIZE = 1024 * 1024 * 1024;
        public const uint ENET_PROTOCOL_MAXIMUM_PEER_ID = 0xfff;
        public const uint ENET_VERSION = (1 << 16) | (3 << 8) | (6 << 0);

        #region Address Functions
        [DllImport ("enet")]
        public static extern int enet_address_set_host(ref ENetAddress address, byte* hostName);

        [DllImport ("enet")]
        public static extern int enet_address_set_host(ref ENetAddress address, byte[] hostName);

        [DllImport ("enet")]
        public static extern int enet_address_get_host(ref ENetAddress address, byte* hostName, IntPtr nameLength);

        [DllImport ("enet")]
        public static extern int enet_address_get_host(ref ENetAddress address, byte[] hostName, IntPtr nameLength);

        [DllImport ("enet")]
        public static extern int enet_address_get_host_ip(ref ENetAddress address, byte* hostIP, IntPtr ipLength);

        [DllImport ("enet")]
        public static extern int enet_address_get_host_ip(ref ENetAddress address, byte[] hostIP, IntPtr ipLength);
        #endregion

        #region Global Functions
        [DllImport ("enet")]
        public static extern int enet_initialize_with_callbacks(uint version, ref ENetCallbacks inits);

        [DllImport ("enet")]
        public static extern int enet_deinitialize();
        #endregion

        #region Host Functions
        [DllImport ("enet")]
        public static extern void enet_host_bandwidth_limit(ENetHost* host, uint incomingBandwidth, uint outgoingBandwidth);

        [DllImport ("enet")]
        public static extern void enet_host_broadcast(ENetHost* host, byte channelID, ENetPacket* packet);

        [DllImport ("enet")]
        public static extern void enet_host_channel_limit(ENetHost* host, IntPtr channelLimit);

        [DllImport ("enet")]
        public static extern int enet_host_check_events(ENetHost* host, out ENetEvent @event);

        [DllImport ("enet")]
        public static extern ENetPeer* enet_host_connect(ENetHost* host, ref ENetAddress address, IntPtr channelCount, uint data);

        [DllImport ("enet")]
        public static extern void enet_host_compress(ENetHost* host, ENetCompressor* compressor);

        [DllImport ("enet")]
        public static extern int enet_host_compress_with_range_encoder(ENetHost* host);

        [DllImport ("enet")]
        public static extern ENetHost* enet_host_create(ENetAddress* address,
            IntPtr peerLimit, IntPtr channelLimit, uint incomingBandwidth, uint outgoingBandwidth);

        [DllImport ("enet")]
        public static extern ENetHost* enet_host_create(ref ENetAddress address,
            IntPtr peerLimit, IntPtr channelLimit, uint incomingBandwidth, uint outgoingBandwidth);

        [DllImport ("enet")]
        public static extern void enet_host_destroy(ENetHost* host);

        [DllImport ("enet")]
        public static extern void enet_host_flush(ENetHost* host);

        [DllImport ("enet")]
        public static extern int enet_host_service(ENetHost* host, ENetEvent* @event, uint timeout);

        [DllImport ("enet")]
        public static extern int enet_host_service(ENetHost* host, out ENetEvent @event, uint timeout);
        #endregion

        #region Miscellaneous Functions
        [DllImport ("enet")]
        public static extern uint enet_time_get();

        [DllImport ("enet")]
        public static extern void enet_time_set(uint newTimeBase);
        #endregion

        #region Packet Functions
        [DllImport ("enet")]
        public static extern ENetPacket* enet_packet_create(IntPtr data, IntPtr dataLength, PacketFlags flags);

        [DllImport ("enet")]
        public static extern void enet_packet_destroy(ENetPacket* packet);

        [DllImport ("enet")]
        public static extern int enet_packet_resize(ENetPacket* packet, IntPtr dataLength);
        #endregion

        #region Peer Functions
        [DllImport ("enet")]
        public static extern void enet_peer_disconnect(ENetPeer* peer, uint data);

        [DllImport ("enet")]
        public static extern void enet_peer_disconnect_now(ENetPeer* peer, uint data);

        [DllImport ("enet")]
        public static extern void enet_peer_disconnect_later(ENetPeer* peer, uint data);

        [DllImport ("enet")]
        public static extern void enet_peer_ping(ENetPeer* peer);

        [DllImport ("enet")]
        public static extern void enet_peer_ping_interval(ENetPeer* peer, uint pingInterval);

        [DllImport ("enet")]
        public static extern ENetPacket* enet_peer_receive(ENetPeer* peer, out byte channelID);

        [DllImport ("enet")]
        public static extern void enet_peer_reset(ENetPeer* peer);

        [DllImport ("enet")]
        public static extern int enet_peer_send(ENetPeer* peer, byte channelID, ENetPacket* packet);

        [DllImport ("enet")]
        public static extern void enet_peer_throttle_configure(ENetPeer* peer, uint interval, uint acceleration, uint deceleration);

        [DllImport ("enet")]
        public static extern void enet_peer_timeout(ENetPeer* peer, uint timeoutLimit, uint timeoutMinimum, uint timeoutMaximum);
        #endregion

        #region C# Utility
        public static bool memcmp(byte[] s1, byte[] s2)
        {
            if (s1 == null || s2 == null) { throw new ArgumentNullException(); }
            if (s1.Length != s2.Length) { return false; }

            for (int i = 0; i < s1.Length; i++) { if (s1[i] != s2[i]) { return false; } }
            return true;
        }

        public static int strlen(byte[] s)
        {
            if (s == null) { throw new ArgumentNullException(); }

            int i;
            for (i = 0; i < s.Length && s[i] != 0; i++) ;
            return i;
        }
        #endregion
    }
}
