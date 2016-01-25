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
using System.Runtime.InteropServices;
using ENet.Native;

namespace ENet
{
    /// <summary>
    /// Contains data received or to be sent.
    /// </summary>
    public unsafe partial struct Packet : IDisposable, IEquatable<Packet>
    {
        ENetPacket* _packet;

        static Packet()
        {
            InitializeUserData();
        }

        /// <summary>
        /// Initializes a new packet based on a native C packet.
        /// </summary>
        /// <param name="packet">The native C packet.</param>
        public Packet(ENetPacket* packet)
        {
            _packet = packet;
        }

        internal void CheckInitialized()
        {
            if (NativeData == null) { throw new InvalidOperationException("Not initialized."); }
        }

        public override bool Equals(object obj)
        {
            return obj is Packet && Equals((Packet)obj);
        }

        public bool Equals(Packet other)
        {
            return NativeData == other.NativeData;
        }

        public override int GetHashCode()
        {
            return (int)(long)NativeData; // ENet types are malloc'ed. They do not move.
        }

        /// <summary>
        /// Initializes a new packet.
        /// </summary>
        /// <param name="data">The data the packet will contain.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The packet is already initialized.</exception>
        /// <exception cref="ENetException">Packet creation failed.</exception>
        public void Initialize(byte[] data)
        {
            Initialize(data, PacketFlags.None);
        }

        /// <summary>
        /// Initializes a new packet, with the given flags.
        /// </summary>
        /// <param name="data">The data the packet will contain.</param>
        /// <param name="flags">The flags the packet will use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The packet is already initialized.</exception>
        /// <exception cref="ENetException">Packet creation failed.</exception>
        public void Initialize(byte[] data, PacketFlags flags)
        {
            if (data == null) { throw new ArgumentNullException("data"); }
            Initialize(data, 0, data.Length, flags);
        }

        /// <summary>
        /// Initializes a new packet from data, at the given offset, and of the given length.
        /// </summary>
        /// <param name="data">An array containing the data the packet will contain.</param>
        /// <param name="offset">The offset of the first byte of data.</param>
        /// <param name="length">The length of the data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> and/or <paramref name="length"/> are out of range.</exception>
        /// <exception cref="InvalidOperationException">The packet is already initialized.</exception>
        /// <exception cref="ENetException">Packet creation failed.</exception>
        public void Initialize(byte[] data, int offset, int length)
        {
            Initialize(data, offset, length, PacketFlags.None);
        }

        /// <summary>
        /// Initializes a new packet from data at the given offset, of the given length, and with the given flags.
        /// </summary>
        /// <param name="data">An array containing the data the packet will contain.</param>
        /// <param name="offset">The offset of the first byte of data.</param>
        /// <param name="length">The length of the data.</param>
        /// <param name="flags">The flags the packet will use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> and/or <paramref name="length"/> are out of range.</exception>
        /// <exception cref="InvalidOperationException">The packet is already initialized.</exception>
        /// <exception cref="ENetException">Packet creation failed.</exception>
        public void Initialize(byte[] data, int offset, int length, PacketFlags flags)
        {
            if (data == null) { throw new ArgumentNullException("data"); }
            if (offset < 0 || length < 0 || length > data.Length - offset) { throw new ArgumentOutOfRangeException(); }
            fixed (byte* bytes = data) { Initialize((IntPtr)(bytes + offset), length, flags); }
        }

        /// <summary>
        /// Initializes a new packet from data, of the given length.
        /// </summary>
        /// <param name="data">A pointer to the first byte of data.</param>
        /// <param name="length">The length of the data.</param>
        /// <exception cref="InvalidOperationException">The packet is already initialized.</exception>
        /// <exception cref="ENetException">Packet creation failed.</exception>
        public void Initialize(IntPtr data, int length)
        {
            Initialize(data, length, PacketFlags.None);
        }

        /// <summary>
        /// Initializes a new packet from data, of the given length, and with the given flags.
        /// </summary>
        /// <param name="data">A pointer to the first byte of data.</param>
        /// <param name="length">The length of the data.</param>
        /// <param name="flags">The flags the packet will use.</param>
        /// <exception cref="InvalidOperationException">The packet is already initialized.</exception>
        /// <exception cref="ENetException">Packet creation failed.</exception>
        public void Initialize(IntPtr data, int length, PacketFlags flags)
        {
            if (NativeData != null) { throw new InvalidOperationException("Already initialized."); }

            NativeData = ENetApi.enet_packet_create(data, (IntPtr)length, flags);
            if (NativeData == null) { throw new ENetException("Packet creation call failed."); }
        }

        /// <summary>
        /// Copies the packet data into an array.
        /// </summary>
        /// <param name="array">The array to copy into.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        public void CopyTo(byte[] array)
        {
            if (array == null) { throw new ArgumentNullException("array"); }
            CopyTo(array, 0, Length, 0);
        }

        /// <summary>
        /// Copies part of the packet data into an array.
        /// </summary>
        /// <param name="array">The array to copy into.</param>
        /// <param name="arrayIndex">The target array index at which to begin copying.</param>
        /// <param name="count">The number of bytes to copy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> and/or <paramref name="count"/> are out of range.</exception>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        public void CopyTo(byte[] array, int arrayIndex, int count)
        {
            CopyTo(array, arrayIndex, count, 0);
        }

        /// <summary>
        /// Copies part of the packet data into an array.
        /// </summary>
        /// <param name="array">The array to copy into.</param>
        /// <param name="arrayIndex">The target array index at which to begin copying.</param>
        /// <param name="count">The number of bytes to copy.</param>
        /// <param name="sourceIndex">The index into the packet at which to begin copying.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/>, <paramref name="count"/>, and/or <paramref name="sourceIndex"/> are out of range.</exception>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        public void CopyTo(byte[] array, int arrayIndex, int count, int sourceIndex)
        {
            if (array == null) { throw new ArgumentNullException("array"); }
            if (sourceIndex < 0 || arrayIndex < 0 || count < 0 || count > array.Length - arrayIndex) { throw new ArgumentOutOfRangeException(); }

            CheckInitialized();
            if (count > Length - sourceIndex) { throw new ArgumentOutOfRangeException(); }
            if (count > 0)
            {
                IntPtr srcPtr = (IntPtr)((byte*)Data + sourceIndex);
                Marshal.Copy(srcPtr, array, arrayIndex, count);
            }
        }

        /// <summary>
        /// Gets all bytes of the packet data.
        /// </summary>
        /// <returns>The packet data.</returns>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        public byte[] GetBytes()
        {
            CheckInitialized();
            byte[] array = new byte[Length]; CopyTo(array); return array;
        }

        /// <summary>
        /// Destroys the ENet packet.
        /// </summary>
        public void Dispose()
        {
            if (NativeData != null)
            {
                if (NativeData->referenceCount == IntPtr.Zero) { ENetApi.enet_packet_destroy(NativeData); }
                NativeData = null;
            }
        }

        /// <summary>
        /// Resizes the packet.
        /// </summary>
        /// <param name="length">The new packet length.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is negative.</exception>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        public void Resize(int length)
        {
            if (length < 0) { throw new ArgumentOutOfRangeException("length"); }
            CheckInitialized(); int ret = ENetApi.enet_packet_resize(NativeData, (IntPtr)length);
            if (ret < 0) { throw new OutOfMemoryException("Packet resizing failed."); }
        }

        /// <summary>
        /// Gets a pointer to the packet data.
        /// </summary>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        public IntPtr Data
        {
            get { CheckInitialized(); return NativeData->data; }
        }

        /// <summary>
        /// Gets the packet flags.
        /// </summary>
        public PacketFlags Flags
        {
            get { CheckInitialized(); return NativeData->flags; }
        }

        /// <summary>
        /// Gets the length of the packet.
        /// </summary>
        /// <exception cref="InvalidOperationException">The packet is not initialized, or is 2GB or larger.</exception>
        public int Length
        {
            get
            {
                CheckInitialized();
                if ((ulong)NativeData->dataLength > (uint)int.MaxValue) { throw new InvalidOperationException("The packet is 2GB or larger."); }
                return (int)NativeData->dataLength;
            }
        }

        /// <summary>
        /// Gets or sets the native C packet.
        /// </summary>
        public ENetPacket* NativeData
        {
            get { return _packet; }
            set { _packet = value; }
        }

        /// <summary>
        /// Gets or sets the reference count.
        /// If you want to keep a packet around that you are giving to ENet, increment this.
        /// When you are finished with it, decrement this and call Dispose().
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value may not be negative.</exception>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        public int ReferenceCount
        {
            get
            {
                CheckInitialized();
                return NativeData->referenceCount.ToInt32();
            }

            set
            {
                if (value < 0) { throw new ArgumentOutOfRangeException(); }
                CheckInitialized(); NativeData->referenceCount = (IntPtr)value;
            }
        }

        /// <summary>
        /// Returns true if the packet is initialized.
        /// </summary>
        public bool IsInitialized
        {
            get { return NativeData != null; }
        }

        public static bool operator ==(Packet packet1, Packet packet2)
        {
            return packet1.Equals(packet2);
        }

        public static bool operator !=(Packet packet1, Packet packet2)
        {
            return !packet1.Equals(packet2);
        }
    }
}
