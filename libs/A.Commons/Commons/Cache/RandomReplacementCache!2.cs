namespace A.Commons.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    using System.Runtime.InteropServices;

    public class RandomReplacementCache<TKey, TValue> : ICache<TKey, TValue>
    {
        private Dictionary<TKey, TValue> cache;

        public RandomReplacementCache(int size)
        {
            this.cache = new Dictionary<TKey, TValue>();
            this.Size = size;
        }

        public void ClearAll()
        {
            this.cache.Clear();
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
                TValue local = default(TValue);
                if (this.cache.TryGetValue(key, out local))
                {
                    this.cache.Remove(key);
                }
                return local;
            }
        }

        public void Set(TKey key, TValue value, int? timeoutSecond = new int?())
        {
            lock (this.cache)
            {
                this.cache.Add(key, value);
                if (this.cache.Count > this.Size)
                {
                    int index = new Random().Next(0, this.cache.Count);
                    this.cache.Remove(this.cache.Keys.ToArray<TKey>()[index]);
                }
            }
        }

        public bool TryGet(TKey key, out TValue value)
        {
            lock (this.cache)
            {
                return this.cache.TryGetValue(key, out value);
            }
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

