namespace A.Commons.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    public class MemoryCache<TKey, TValue> : ICache<TKey, TValue>
    {
        protected Dictionary<TKey, TValue> container;

        public MemoryCache()
        {
            this.container = new Dictionary<TKey, TValue>();
        }

        public void ClearAll()
        {
            List<TKey> list = this.container.Keys.ToList<TKey>();
            foreach (TKey local in list)
            {
                this.Remove(local);
            }
        }

        public TValue Get(TKey key)
        {
            TValue local = default(TValue);
            this.container.TryGetValue(key, out local);
            return local;
        }

        public TValue Remove(TKey key)
        {
            TValue local = default(TValue);
            lock (this.container)
            {
                bool flag = this.container.TryGetValue(key, out local);
                this.container.Remove(key);
            }
            return local;
        }

        public void Set(TKey key, TValue value, int? timeoutSecond = new int?())
        {
            lock (this.container)
            {
                if (!this.container.ContainsKey(key))
                {
                    this.container.Add(key, value);
                }
                else
                {
                    this.container[key] = value;
                }
            }
        }

        public bool TryGet(TKey key, out TValue value)
        {
            TValue local = default(TValue);
            bool flag = this.container.TryGetValue(key, out local);
            value = local;
            return flag;
        }

        public long Count
        {
            get
            {
                return (long) this.container.Count;
            }
        }

        public TKey[] Keys
        {
            get
            {
                return this.container.Keys.ToArray<TKey>();
            }
        }
    }
}

