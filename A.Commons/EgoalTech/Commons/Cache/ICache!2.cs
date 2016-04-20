namespace EgoalTech.Commons.Cache
{
    using System;
    using System.Runtime.InteropServices;

    public interface ICache<TKey, TValue>
    {
        void ClearAll();
        TValue Get(TKey key);
        TValue Remove(TKey key);
        void Set(TKey key, TValue value, int? timeoutSecond = new int?());
        bool TryGet(TKey key, out TValue value);

        long Count { get; }
    }
}

