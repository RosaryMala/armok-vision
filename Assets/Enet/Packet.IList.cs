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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ENet
{
    partial struct Packet : IList<byte>
    {
        /// <summary>
        /// Adds a byte to the end of the packet.
        /// </summary>
        /// <param name="value">The value of the byte.</param>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        /// <remarks>
        /// Packet resizing behavior is less than optimal. So, in performance-critical applications, it's
        /// presently a good idea to initialize the packet with a final byte array instead of using this method.
        /// </remarks>
        public void Add(byte value)
        {
            Insert(Length, value);
        }

        /// <summary>
        /// Sets the packet length to zero.
        /// </summary>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        public void Clear()
        {
            Resize(0);
        }

        /// <summary>
        /// Checks if the packet contains a particular byte.
        /// </summary>
        /// <param name="value">The value to look for.</param>
        /// <returns>True if the packet contains the byte.</returns>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        public bool Contains(byte value)
        {
            return IndexOf(value) != -1;
        }

        /// <summary>
        /// Copies part of the packet data into an array.
        /// </summary>
        /// <param name="array">The array to copy into.</param>
        /// <param name="arrayIndex">The target array index at which to begin copying.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is out of range.</exception>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        public void CopyTo(byte[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex, Length, 0);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the bytes of the packet.
        /// </summary>
        /// <returns>An enumerator.</returns>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        public IEnumerator<byte> GetEnumerator()
        {
            CheckInitialized();

            for (int i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Checks if the packet contains a particular byte, and if so, returns the index.
        /// </summary>
        /// <param name="value">The value to look for.</param>
        /// <returns>The index of the byte, or -1.</returns>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        public int IndexOf(byte value)
        {
            for (int i = 0; i < Length; i++)
            {
                if (this[i] == value) { return i; }
            }

            return -1;
        }

        /// <summary>
        /// Inserts a byte in the packet.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="value">The value of the byte.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        /// <remarks>
        /// Packet resizing behavior is less than optimal. So, in performance-critical applications, it's
        /// presently a good idea to initialize the packet with a final byte array instead of using this method.
        /// </remarks>
        public void Insert(int index, byte value)
        {
            if (index < 0 || index > Length) { throw new ArgumentOutOfRangeException("index"); }

            Resize(checked(Length + 1));
            for (int i = Length - 1; i > index; i--)
            {
                this[i] = this[i - 1];
            }
            this[index] = value;
        }

        /// <summary>
        /// Removes the first byte in the packet with the specified value.
        /// </summary>
        /// <param name="value">The value of the byte to remove.</param>
        /// <returns>True if a byte was found and removed.</returns>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        public bool Remove(byte value)
        {
            int index = IndexOf(value);
            if (index == -1) { return false; }
            RemoveAt(index); return true;
        }

        /// <summary>
        /// Removes the byte at the specified index from the packet.
        /// </summary>
        /// <param name="index">The index of the byte to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Length) { throw new ArgumentOutOfRangeException("index"); }

            for (int i = index; i < Length - 1; i++)
            {
                this[i] = this[i + 1];
            }
            Resize(Length - 1);
        }

        /// <summary>
        /// Gets or sets the byte at the specified index.
        /// </summary>
        /// <param name="index">The index of the byte.</param>
        /// <returns>The byte value.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        /// <exception cref="InvalidOperationException">The packet is not initialized.</exception>
        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= Length) { throw new ArgumentOutOfRangeException("index"); }
                return Marshal.ReadByte(Data, index);
            }

            set
            {
                if (index < 0 || index >= Length) { throw new ArgumentOutOfRangeException("index"); }
                Marshal.WriteByte(Data, index, value);
            }
        }

        int ICollection<byte>.Count
        {
            get { return Length; }
        }

        bool ICollection<byte>.IsReadOnly
        {
            get { return false; }
        }
    }
}
