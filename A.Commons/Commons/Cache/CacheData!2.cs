namespace EgoalTech.Commons.Cache
{
    using System;
    using System.Runtime.CompilerServices;

    public class CacheData<TKey, TValue>
    {
        public TKey Key { get; set; }

        public DateTime OperateTime { get; set; }

        public DateTime Timeout { get; set; }

        public TValue Value { get; set; }
    }
}

