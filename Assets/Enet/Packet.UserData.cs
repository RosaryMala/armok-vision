#region License
/*
ENet for C#
Copyright (c) 2013 James F. Bellinger <jfb@zer7.com>

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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace ENet
{
    /// <summary>
    /// Called when ENet is finished with a packet and it is about to be freed from memory.
    /// </summary>
    /// <param name="packet">The packet.</param>
    public delegate void PacketFreeCallback(Packet packet);

    unsafe partial struct Packet
    {
        static Native.ENetPacket.freeCallback_cb _freeCallbackDelegate;
        static IntPtr _freeCallbackFunctionPointer;
        static object _freeCallbackKey;
        static Dictionary<Packet, Dictionary<object, object>> _userData;
        static object _userDataDefaultKey;

        static void InitializeUserData()
        {
            _freeCallbackDelegate = FreeCallbackHandler;
            _freeCallbackFunctionPointer = Marshal.GetFunctionPointerForDelegate(_freeCallbackDelegate);
            _freeCallbackKey = new object();
            _userData = new Dictionary<Packet, Dictionary<object, object>>();
            _userDataDefaultKey = new object();
        }

        static void FreeCallbackHandler(Native.ENetPacket* native)
        {
            Packet packet = new Packet(native);

            lock (_userData)
            {
                try
                {
                    PacketFreeCallback callback = (PacketFreeCallback)packet.GetUserData(_freeCallbackKey);

                    if (callback != null)
                    {
                        try
                        {
                            Monitor.Exit(_userData);
                            callback(packet);
                        }
                        finally
                        {
                            Monitor.Enter(_userData);
                        }
                    }
                }
                finally
                {
                    _userData.Remove(packet);
                }
            }
        }

        /// <summary>
        /// Occurs when ENet is finished with a packet and it is about to be freed from memory.
        /// </summary>
        public event PacketFreeCallback Freed
        {
            add
            {
                lock (_userData)
                {
                    SetUserData(_freeCallbackKey, (PacketFreeCallback)GetUserData(_freeCallbackKey) + value);
                }
            }

            remove
            {
                lock (_userData)
                {
                    SetUserData(_freeCallbackKey, (PacketFreeCallback)GetUserData(_freeCallbackKey) - value);
                }
            }
        }

        /// <summary>
        /// Gets the user data associated with this packet.
        /// </summary>
        /// <returns></returns>
        public object GetUserData()
        {
            return GetUserData(null);
        }

        /// <summary>
        /// Gets the user data associated with this packet and a particular key.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <returns>The user data.</returns>
        public object GetUserData(object key)
        {
            if (key == null) { key = _userDataDefaultKey; }

            lock (_userData)
            {
                Dictionary<object, object> packetUserData;
                if (!_userData.TryGetValue(this, out packetUserData)) { return null; }

                object value;
                if (!packetUserData.TryGetValue(key, out value)) { return null; }
                return value;
            }
        }

        /// <summary>
        /// Associates user data with this packet.
        /// </summary>
        /// <param name="value"></param>
        public void SetUserData(object value)
        {
            SetUserData(null, value);
        }

        /// <summary>
        /// Associates user data with this packet and a particular key.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <param name="value">The user data.</param>
        public void SetUserData(object key, object value)
        {
            if (key == null) { key = _userDataDefaultKey; }

            lock (_userData)
            {
                CheckInitialized();
                if (NativeData->freeCallback != _freeCallbackFunctionPointer)
                {
                    if (NativeData->freeCallback != IntPtr.Zero) { throw new InvalidOperationException("An unknown free callback is set on this packet."); }
                    NativeData->freeCallback = _freeCallbackFunctionPointer;
                }

                Dictionary<object, object> packetUserData;
                if (!_userData.TryGetValue(this, out packetUserData))
                {
                    if (value == null) { return; }
                    _userData[this] = packetUserData = new Dictionary<object, object>();
                }

                if (value == null)
                {
                    packetUserData.Remove(key);
                }
                else
                {
                    packetUserData[key] = value;
                }
            }
        }
    }
}
