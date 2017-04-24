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

using ENet.Native;

namespace ENet
{
    /// <summary>
    /// Represents an event from the <see cref="Host.Service(int, out Event)"/> method.
    /// </summary>
    public unsafe struct Event
    {
        ENetEvent _event;

        /// <summary>
        /// Initializes an event based on a native C event.
        /// </summary>
        /// <param name="event">The native C event.</param>
        public Event(ENetEvent @event)
        {
            this = new Event() { NativeData = @event };
        }

        /// <summary>
        /// Gets the channel ID.
        /// </summary>
        public byte ChannelID
        {
            get { return NativeData.channelID; }
        }

        /// <summary>
        /// Gets the data associated with the event.
        /// </summary>
        public int Data
        {
            get { return (int)NativeData.data; }
        }

        /// <summary>
        /// Gets or sets the native C event.
        /// </summary>
        public ENetEvent NativeData
        {
            get { return _event; }
            set { _event = value; }
        }

        /// <summary>
        /// Gets the packet associated with the event.
        /// </summary>
        public Packet Packet
        {
            get { return new Packet(NativeData.packet); }
        }

        /// <summary>
        /// Gets the peer associated with the event.
        /// </summary>
        public Peer Peer
        {
            get { return new Peer(NativeData.peer); }
        }

        /// <summary>
        /// Gets the event type.
        /// </summary>
        public EventType Type
        {
            get { return NativeData.type; }
        }
    }
}
