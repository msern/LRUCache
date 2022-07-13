using LRUCache.Models;
using System.Collections.Generic;

namespace LRUCache
{
    public class LRUCache<K, V> : ICache<K, V>
    {
        public long Capacity { get; }

        private readonly Dictionary<K, Node<K, V>> _map;
        private readonly Node<K, V> _head;
        private readonly Node<K, V> _tail;

        private static LRUCache<K, V> _instance = null;
        private static readonly object _padlock = new object();

        public static LRUCache<K, V> Instance
        {
            get 
            {
                lock(_padlock)
                {
                    if (_instance == null)
                    {
                        // capacity could be setup through an additional init method
                        _instance = new LRUCache<K, V>(16);
                    }
                    return _instance;
                }
            }
        }

        private LRUCache(long capacity)
        {
            _map = new Dictionary<K, Node<K, V>>();

            _head = new Node<K, V>();
            _tail = new Node<K, V>();
            _tail.Next = _head;
            _head.Prev = _tail;

            Capacity = capacity;
        }

        public V Get(K key)
        {
            lock (_padlock)
            {
                if (!_map.ContainsKey(key))
                {
                    return default;
                }

                var node = _map[key];

                RemoveNode(node);
                InsertNode(node);

                return node.Value;
            }
        }

        public void Put(K key, V value)
        {
            lock (_padlock)
            {
                if (_map.ContainsKey(key))
                {
                    RemoveNode(_map[key]);
                }
                else if (_map.Count == Capacity)
                {
                    RemoveNode(_tail.Next);
                }

                var n = new Node<K, V>(key, value);
                InsertNode(n);
            }
        }

        private void InsertNode(Node<K, V> n)
        {
            _map.Add(n.Key, n);

            var headPrev = _head.Prev;
            _head.Prev = n;
            n.Next = _head;

            n.Prev = headPrev;
            headPrev.Next = n;
        }

        private void RemoveNode(Node<K, V> n)
        {
            _map.Remove(n.Key);
            n.Prev.Next = n.Next;
            n.Next.Prev = n.Prev;
        }
    }
}
