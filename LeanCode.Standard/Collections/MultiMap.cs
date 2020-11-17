using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace LeanCode.Standard.Collections
{
    public class MultiMap<K, V>
    {
        // 1
        ConcurrentDictionary<K, HashSet<V>> _dictionary = null;

        public MultiMap()
        {
            _dictionary = new ConcurrentDictionary<K, HashSet<V>>();
        }

        public MultiMap(IEqualityComparer<K> comparer)
        {
            _dictionary = new ConcurrentDictionary<K, HashSet<V>>(comparer);
        }

        // 2
        public void Add(K key, V value)
        {
            HashSet<V> set;
            if (this._dictionary.TryGetValue(key, out set))
            {
                // 2A.
                set.Add(value);
            }
            else
            {
                // 2B.
                set = new HashSet<V>();
                set.Add(value);
                this._dictionary[key] = set;
            }
        }

        // remove value if it exists; do nothing otherwise
        public void Remove(K key, V value)
        {
            HashSet<V> set;
            if (this._dictionary.TryGetValue(key, out set))
            {
                set.Remove(value);
            }
        }

        // remove value from all keys where it is listed
        public void RemoveAll(V value)
        {
            var keys = _dictionary.Keys.ToArray();

            foreach (var k in keys)
            {
                Remove(k, value);
            }
        }

        // check whether value already exists
        public bool Contains(K key, V value)
        {
            System.Collections.Generic.HashSet<V> set;
            if (this._dictionary.TryGetValue(key, out set))
            {
                return set.Contains(value);
            }
            else return false;
        }

        /// <summary>
        ///   Checks if the key specified exists in the collection
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(K key)
        {
            return this._dictionary.ContainsKey(key);
        }

        // 3
        public IEnumerable<K> Keys
        {
            get
            {
                return this._dictionary.Keys;
            }
        }

        // 4
        public ISet<V> this[K key]
        {
            get
            {
                HashSet<V> set;
                if (this._dictionary.TryGetValue(key, out set))
                {
                    return set;
                }
                else
                {
                    return new HashSet<V>();
                }
            }
        }

        public void Clear()
        {
            this._dictionary.Clear();
        }
    }
}
