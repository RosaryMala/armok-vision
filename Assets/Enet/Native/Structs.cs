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

#pragma warning disable 1591

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace ENet.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ENetAddress
    {
        public uint host;
        public ushort port;

        /// <exception cref="ArgumentNullException">The address is null.</exception>
        /// <exception cref="ArgumentException">The address is not IPv4.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The port is less than 0 or greater than 65535.</exception>
        public static explicit operator ENetAddress(IPEndPoint endPoint)
        {
            if (endPoint == null) { throw new ArgumentNullException(); }
            if (endPoint.AddressFamily != AddressFamily.InterNetwork) { throw new ArgumentException(); }
            if (endPoint.Port < 0 || endPoint.Port > 65535) { throw new ArgumentOutOfRangeException(); }

            return new ENetAddress()
            {
                host = endPoint.Address == IPAddress.Any ? 0 : BitConverter.ToUInt32(endPoint.Address.GetAddressBytes(), 0),
                port = (ushort)endPoint.Port
            };
        }

        public static implicit operator IPEndPoint(ENetAddress address)
        {
            return new IPEndPoint(address.host, address.port);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ENetCallbacks
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr malloc_cb(IntPtr size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void free_cb(IntPtr memory);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void no_memory_cb();

        public IntPtr malloc, free, no_memory;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ENetCompressor
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr compress_cb(IntPtr context, IntPtr inBuffers, IntPtr inBufferCount, IntPtr inLimit, IntPtr outData, IntPtr outLimit);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr decompress_cb(IntPtr context, IntPtr inData, IntPtr inLimit, IntPtr outData, IntPtr outLimit);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void destroy_cb(IntPtr context);

        public IntPtr context;
        public IntPtr compress, decompress, destroy;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ENetEvent
    {
        public EventType type;
        public ENetPeer* peer;
        public byte channelID;
        public uint data;
        public ENetPacket* packet;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ENetHost
    {

    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ENetListNode
    {
        public ENetListNode* next, previous;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ENetPacket
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void freeCallback_cb(ENetPacket* packet);

        public IntPtr referenceCount;
        public PacketFlags flags;
        public IntPtr data;
        public IntPtr dataLength;
        public IntPtr freeCallback;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ENetPeer
    {
        public ENetListNode dispatchList;
        public ENetHost* host;
        public ushort outgoingPeerID;
        public ushort incomingPeerID;
        public uint connectID;
        public byte outgoingSessionID;
        public byte incomingSessionID;

        uint addressHost; // https://bugzilla.xamarin.com/show_bug.cgi?id=11899
        ushort addressPort;
        public ENetAddress address
        {
            get { return new ENetAddress() { host = addressHost, port = addressPort }; }
            set { addressHost = value.host; addressPort = value.port; }
        }

        public IntPtr data;
        public PeerState state;
    }
}
