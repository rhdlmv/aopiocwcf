namespace A.Commons.Cache
{
    using System;
    

    public class CacheData<TKey, TValue>
    {
        public TKey Key { get; set; }

        public DateTime OperateTime { get; set; }

        public DateTime Timeout { get; set; }

        public TValue Value { get; set; }
    }
}

