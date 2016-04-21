namespace EgoalTech.Commons.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class TimeoutCache<TKey, TValue> : ICache<TKey, TValue>
    {
        private MemoryCache<TKey, CacheData<TKey, TValue>> container;
        private DateTime lastScanningTime;

        public TimeoutCache()
        {
            this.lastScanningTime = DateTime.Now;
            this.container = new MemoryCache<TKey, CacheData<TKey, TValue>>();
            this.defaultTimeout = Math.Max(60, this.defaultTimeout);
            this.maxTimeout = Math.Max(0x15180, this.maxTimeout);
            this.scanningInterval = 300;
        }

        public void ClearAll()
        {
            this.container.ClearAll();
        }

        public TValue Get(TKey key)
        {
            TValue local = default(TValue);
            this.TryGet(key, out local);
            return local;
        }

        public TValue Remove(TKey key)
        {
            this.RemoveExpiredValues();
            TValue local = default(TValue);
            CacheData<TKey, TValue> data = null;
            if (this.container.TryGet(key, out data))
            {
                local = data.Value;
                data.Timeout = DateTime.Now.AddDays(-1.0);
            }
            return local;
        }

        public void RemoveExpiredValues()
        {
            if ((this.lastScanningTime.AddSeconds((double) this.scanningInterval) < DateTime.Now) && (this.container.Count > 0L))
            {
                lock (this.container)
                {
                    IEnumerable<TKey> keys = this.container.Keys;
                    foreach (TKey local in keys)
                    {
                        CacheData<TKey, TValue> data = this.container.Get(local);
                        if ((data != null) && (data.Timeout < DateTime.Now))
                        {
                            this.container.Remove(local);
                        }
                    }
                }
            }
        }

        protected void Set(CacheData<TKey, TValue> data)
        {
            this.RemoveExpiredValues();
            DateTime time = DateTime.Now.AddSeconds((double) this.maxTimeout);
            if (data.Timeout > time)
            {
                data.Timeout = time;
            }
            lock (this.container)
            {
                CacheData<TKey, TValue> data2 = null;
                this.container.TryGet(data.Key, out data2);
                if ((data2 == null) || (data2.OperateTime <= data.OperateTime))
                {
                    this.container.Set(data.Key, data, null);
                }
            }
        }

        public void Set(TKey key, TValue value, int? timeoutSecond = new int?())
        {
            CacheData<TKey, TValue> data2 = new CacheData<TKey, TValue> {
                Key = key,
                Value = value
            };
            int? nullable = timeoutSecond;
            data2.Timeout = DateTime.Now.AddSeconds(nullable.HasValue ? ((double) nullable.GetValueOrDefault()) : ((double) this.defaultTimeout));
            data2.OperateTime = DateTime.Now;
            CacheData<TKey, TValue> data = data2;
            this.Set(data);
        }

        public bool TryGet(TKey key, out TValue value)
        {
            bool flag = false;
            value = default(TValue);
            CacheData<TKey, TValue> data = null;
            if (this.container.TryGet(key, out data) && (data.Timeout > DateTime.Now))
            {
                value = data.Value;
                flag = true;
            }
            return flag;
        }

        public long Count
        {
            get
            {
                return this.container.Count;
            }
        }

        public virtual int defaultTimeout { get; set; }

        public virtual int maxTimeout { get; set; }

        public virtual int scanningInterval { get; set; }
    }
}

