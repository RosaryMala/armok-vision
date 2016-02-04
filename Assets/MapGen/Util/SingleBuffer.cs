using System;
using System.Threading;
using UnityEngine;

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
        /// Atomically set the current value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Set(T value) {
            this.value = value;
        }

        /// <summary>
        /// Atomically get and remove the current value.
        /// </summary>
        /// <returns>
        /// The current value, which may be null, or null if no value has been
        /// set since the last call to Pop().
        /// </returns>
        public T Pop() {
            // Needed to make the operation atomic.
            return Interlocked.Exchange(ref this.value, null);
        }
    }

    /// <summary>
    /// A non-blocking buffer containing a single struct element.
    /// Writing to the buffer replaces the element, if set.
    /// </summary>
    public struct SingleBufferStruct<T> where T : struct
    {
        // We have to use a lock because it's impossible to atomically modify
        // structs.
        // ...we could use a normal lock, but this is more fun.
        private SpinLock spinLock;
        private T? value;

        /// <summary>
        /// Atomically set the current value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Set(T value) {
            spinLock.Enter();
            this.value = value;
            spinLock.Exit();
        }

        /// <summary>
        /// Atomically get and remove the current value., 
        /// </summary>
        /// <returns>
        /// The current value, which may be null, or null if no value has been
        /// set since the last call to Pop().
        /// </returns>
        public T? Pop() {
            T? result;

            spinLock.Enter();
            result = this.value;
            this.value = null;
            spinLock.Exit();

            return result;
        }
    }

    /// <summary>
    /// A non-blocking buffer that tells whether an event has happened since
    /// the last Pop().
    /// </summary>
    public struct EventBuffer
    {
        // There's no Interlocked.Exchange() method for bools.
        private int value;

        /// <summary>
        /// Set the fact that an event has occured atomically.
        /// </summary>
        public void Set() {
            this.value = 1;
        }

        /// <summary>
        /// Check whether an event has occured atomically.
        /// </summary>
        /// <returns>
        /// true if Set() has been called since the last Pop(), false otherwise.
        /// </returns>
        public bool Pop() {
            return Interlocked.Exchange(ref this.value, 0) == 1;
        }
    }
}

