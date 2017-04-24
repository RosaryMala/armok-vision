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
using System.Runtime.Serialization;

namespace ENet
{
    /// <summary>
    /// This exception is thrown when ENet has an error.
    /// </summary>
    [Serializable]
    public class ENetException : Exception
    {
        /// <summary>
        /// Initializes an ENetException.
        /// </summary>
        public ENetException()
            : base("ENet error.")
        {

        }

        /// <summary>
        /// Initializes an ENetException with a given message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ENetException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes an ENetException with the given message and a reference to an inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ENetException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        /// <summary>
        /// Initializes an ENetException for serialization.
        /// </summary>
        /// <param name="info">The serialized data.</param>
        /// <param name="context">The context for the serialization.</param>
        protected ENetException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
