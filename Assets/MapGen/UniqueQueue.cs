using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Util {
    // Class for storing a queue of elements with unique keys;
    // E.g. map blocks for meshing, where we don't want to
    // mesh the same block twice.
    public sealed class UniqueQueue<Key, Value> {
        private Dictionary<Key, Value> dict;
        private Queue<Key> queue;

        public int Count {
            get {
                int dictCount = dict.Count;
                if (dictCount != queue.Count) {
                    throw new System.InvalidOperationException("Inconsistent state?");
                }
                return dictCount;
            }
        }

        public UniqueQueue() {
            dict = new Dictionary<Key, Value>();
            queue = new Queue<Key>();
        }

        // Push a value and return anything it displaced
        public Value EnqueueAndDisplace(Key key, Value value) {
            if (dict.ContainsKey(key)) {
                // This key is already in the queue.
                Value result = dict[key];
                dict.Remove(key);
                dict[key] = value;
                return result;
            } else {
                // This key isn't in the queue; add it:
                queue.Enqueue(key);
                dict[key] = value;
                return default(Value);
            }
        }

        public Value this[Key key] {
            get {
                if (dict.ContainsKey(key)) {
                    return dict[key];
                } else {
                    return default(Value);
                }
            }
        }

        public Value Dequeue() {
            if (queue.Count == 0) return default(Value);
            Key next = queue.Dequeue();
            if (!dict.ContainsKey(next)) {
                throw new System.InvalidOperationException("Dict doesn't contain queue entry, sky is falling");
            }
            Value result = dict[next];
            dict.Remove(next);
            return result;
        }
    }
}
