#region LICENSE
/*
 The MIT License (MIT)
 Copyright (c) 2015 Grid Protection Alliance

 Permission is hereby granted, free of charge, to any person obtaining a
 copy of this software and associated documentation files (the "Software"),
 to deal in the Software without restriction, including without limitation
 the rights to use, copy, modify, merge, publish, distribute, sub-license,
 and/or sell copies of the Software, and to permit persons to whom the
 Software is furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 DEALINGS IN THE SOFTWARE.
 */
#endregion

using System;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;

// A spin lock, since Unity doesn't provide one.
// Taken from / inspired by: https://github.com/GridProtectionAlliance/gsf
// and modified for Unity compatibility.

namespace Util
{
    /// <summary>
    /// Provides a mutual exclusion lock primitive where a thread trying to
    /// acquire the lock waits in a loop repeatedly checking until the lock
    /// becomes available.
    /// </summary>
    public struct SpinLock
    {
        private int lockState;

        /// <summary>
        /// Gets whether the lock is currently held by any thread.
        /// </summary>
        /// <returns>true if the lock is currently held by any thread; otherwise false.</returns>
        public bool IsHeld
        {
            get
            {
                return this.lockState != 0;
            }
        }

        /// <summary>
        /// Acquires the lock.
        /// </summary>
        public void Enter()
        {
            int count = 0;

            while (Interlocked.CompareExchange(ref lockState, 1, 0) != 0)
            {
                count++;

                if (count <= 5)
                    Thread.SpinWait(25);
                else
                    Thread.Sleep(0);
            }
        }

        /// <summary>
        /// Attempts to acquire the lock.
        /// </summary>
        /// <returns>
        /// Whether the lock has been acquired.
        /// </returns>
        public bool TryEnter()
        {
            return Interlocked.CompareExchange(ref lockState, 1, 0) == 0;
        }

        /// <summary>
        /// Releases the lock.
        /// </summary>
        public void Exit()
        {
            this.lockState = 0;
        }
    }
}