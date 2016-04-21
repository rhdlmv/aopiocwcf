namespace EgoalTech.Commons.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class LRUCache<TKey, TValue> : ICache<TKey, TValue>
    {
        private Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> cache;
        private LinkedList<KeyValuePair<TKey, TValue>> lruList;

        public LRUCache(int size)
        {
            this.cache = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
            this.lruList = new LinkedList<KeyValuePair<TKey, TValue>>();
            this.Size = size;
        }

        public void ClearAll()
        {
            this.cache.Clear();
            this.lruList = new LinkedList<KeyValuePair<TKey, TValue>>();
        }

        public TValue Get(TKey key)
        {
            TValue local;
            this.TryGet(key, out local);
            return local;
        }

        public TValue Remove(TKey key)
        {
            lock (this.cache)
            {
                LinkedListNode<KeyValuePair<TKey, TValue>> node;
                if (this.cache.TryGetValue(key, out node))
                {
                    this.cache.Remove(key);
                    this.lruList.Remove(node);
                }
                if (node != null)
                {
                    return node.Value.Value;
                }
                return default(TValue);
            }
        }

        public void Set(TKey key, TValue value, int? timeoutSecond = new int?())
        {
            lock (this.cache)
            {
                LinkedListNode<KeyValuePair<TKey, TValue>> node;
                if (this.cache.TryGetValue(key, out node))
                {
                    this.lruList.Remove(node);
                    this.lruList.AddFirst(node);
                }
                else
                {
                    node = new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
                    this.cache.Add(key, node);
                    this.lruList.AddFirst(node);
                }
                if (this.cache.Count > this.Size)
                {
                    LinkedListNode<KeyValuePair<TKey, TValue>> last = this.lruList.Last;
                    if (last != null)
                    {
                        this.Remove(last.Value.Key);
                    }
                }
            }
        }

        public bool TryGet(TKey key, out TValue value)
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> node;
            bool flag = false;
            lock (this.cache)
            {
                flag = this.cache.TryGetValue(key, out node);
            }
            if (node != null)
            {
                value = node.Value.Value;
                return flag;
            }
            value = default(TValue);
            return flag;
        }

        public long Count
        {
            get
            {
                return (long) this.cache.Count;
            }
        }

        public int Size { get; set; }
    }
}

