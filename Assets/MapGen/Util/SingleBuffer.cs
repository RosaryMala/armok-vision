using System;
using System.Threading;

namespace Util
{
    /// <summary>
    /// A non-blocking buffer containing a single element.
    /// Writing to the buffer replaces the element, if set.
    /// </summary>
    public struct SingleBuffer<T> where T : class
    {
        private T value;

        /// <summary>
        /// Set the contents of the buffer, returning the current
        /// value in the buffer.
        /// </summary>
        public void Set(T value) {
            this.value = value;
        }

        /// <summary>
        /// Get the contents of the buffer, setting its contents to null.
        /// </summary>
        public T Pop() {
            // Needed to make the operation atomic.
            return Interlocked.Exchange(ref this.value, null);
        }
    }
}

