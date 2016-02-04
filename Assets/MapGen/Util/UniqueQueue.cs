using System.Collections.Generic;

namespace Util
{
    /// <summary>
    /// Class for storing a queue of elements with unique keys;
    /// E.g. map blocks for meshing, where we don't want to
    /// mesh the same block twice.
    /// Thread safe.
    /// </summary>
    public sealed class UniqueQueue<Key, Value> {
        private Dictionary<Key, Value> dict;

        /// <summary>
        /// Gets the number of elements in this queue.
        /// </summary>
        /// <value>The count.</value>
        public int Count {
            get {
                lock (this) {
                    return dict.Count;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Util.UniqueQueue`2"/> class.
        /// </summary>
        /// <param name="capacity">Capacity.</param>
        public UniqueQueue() {
            dict = new Dictionary<Key, Value>();
        }

        /// <summary>
        /// Enqueues a value and returns anything it displaced.
        /// Currently throws an exception if it hits the queue capacity, to
        /// avoid deadlock; there should be a better solution.
        /// </summary>
        /// <returns>The displaced value, or default(Value) if there was none.</returns>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public Value EnqueueAndDisplace(Key key, Value value) {
            lock (this) {
                if (dict.ContainsKey(key)) {
                    // This key is already in the queue.
                    Value result = dict[key];
                    dict.Remove(key);
                    dict[key] = value;
                    return result;
                } else {
                    // This key isn't in the queue; add it:
                    dict[key] = value;
                    return default(Value);
                }
            }
        }

        public Value this[Key key] {
            get {
                lock(this) {
                    if (dict.ContainsKey(key)) {
                        return dict[key];
                    } else {
                        return default(Value);
                    }
                }
            }
        }

        public Value Dequeue() {
            lock(this) {
                if (dict.Count == 0) return default(Value);

                Key next = dict.Keys[0];
                Value result = dict[next];
                dict.Remove(next);
                return result;
            }
        }
    }
}
