namespace EgoalTech.Commons.Cache
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class LFUCache<TKey, TValue> : ICache<TKey, TValue>
    {
        private int age;
        private int agePolicy;
        private Dictionary<TKey, LinkedListNode<CacheNode<TKey, TValue>>> cache;
        private LinkedList<CacheNode<TKey, TValue>> lfuList;
        private int size;

        public LFUCache(int size) : this(size, 0x3e8)
        {
        }

        public LFUCache(int size, int agePolicy)
        {
            this.cache = new Dictionary<TKey, LinkedListNode<CacheNode<TKey, TValue>>>();
            this.lfuList = new LinkedList<CacheNode<TKey, TValue>>();
            this.age = 0;
            this.agePolicy = 0x3e8;
            this.size = size;
        }

        public void ClearAll()
        {
            this.cache.Clear();
            this.lfuList = new LinkedList<CacheNode<TKey, TValue>>();
        }

        public TValue Get(TKey key)
        {
            TValue local;
            this.TryGet(key, out local);
            return local;
        }

        private IEnumerable<LinkedListNode<CacheNode<TKey, TValue>>> IterateFrom(LinkedListNode<CacheNode<TKey, TValue>> node)
        {
            while (node != null)
            {
                yield return node;
                node = node.Next;
            }
        }

        public TValue Remove(TKey key)
        {
            TValue local;
            lock (this.cache)
            {
                if (!this.TryGet(key, out local))
                {
                    this.cache.Remove(key);
                    foreach (CacheNode<TKey, TValue> node in this.lfuList)
                    {
                        if (node.Key.Equals(key))
                        {
                            this.lfuList.Remove(node);
                            return local;
                        }
                    }
                }
                return local;
            }
            return local;
        }

        public void Set(TKey key, TValue value, int? timeoutSecond = new int?())
        {
            lock (this.cache)
            {
                TValue local;
                if (!this.TryGet(key, out local))
                {
                    LinkedListNode<CacheNode<TKey, TValue>> node3;
                    CacheNode<TKey, TValue> node = new CacheNode<TKey, TValue> {
                        Key = key,
                        Data = value,
                        UseCount = 1
                    };
                    if (this.lfuList.Count == this.size)
                    {
                        this.cache.Remove(this.lfuList.First.Value.Key);
                        this.lfuList.RemoveFirst();
                    }
                    LinkedListNode<CacheNode<TKey, TValue>> node2 = this.Nodes.LastOrDefault<LinkedListNode<CacheNode<TKey, TValue>>>(n => n.Value.UseCount < 2);
                    if (node2 == null)
                    {
                        node3 = this.lfuList.AddFirst(node);
                    }
                    else
                    {
                        node3 = this.lfuList.AddAfter(node2, node);
                    }
                    this.cache[key] = node3;
                }
            }
        }

        public bool TryGet(TKey key, out TValue val)
        {
            lock (this.cache)
            {
                LinkedListNode<CacheNode<TKey, TValue>> node3;
                this.age++;
                if (this.age > this.agePolicy)
                {
                    this.age = 0;
                    foreach (LinkedListNode<CacheNode<TKey, TValue>> node in this.cache.Values)
                    {
                        CacheNode<TKey, TValue> local1 = node.Value;
                        local1.UseCount--;
                    }
                }
                if (this.cache.TryGetValue(key, out node3))
                {
                    CacheNode<TKey, TValue> cacheNode = node3.Value;
                    val = cacheNode.Data;
                    cacheNode.UseCount++;
                    LinkedListNode<CacheNode<TKey, TValue>> node4 = this.IterateFrom(node3).Last<LinkedListNode<CacheNode<TKey, TValue>>>(n => n.Value.UseCount <= cacheNode.UseCount);
                    if (node4 != node3)
                    {
                        this.lfuList.Remove(node3);
                        this.lfuList.AddAfter(node4, node3);
                    }
                }
                else
                {
                    val = default(TValue);
                }
                return false;
            }
        }

        public long Count =>
            ((long) this.cache.Count)

        private IEnumerable<LinkedListNode<CacheNode<TKey, TValue>>> Nodes
        {
            get
            {
                for (LinkedListNode<CacheNode<TKey, TValue>> iteratorVariable0 = this.lfuList.First; iteratorVariable0 != null; iteratorVariable0 = iteratorVariable0.Next)
                {
                    yield return iteratorVariable0;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <get_Nodes>d__4 : IEnumerable<LinkedListNode<LFUCache<TKey, TValue>.CacheNode>>, IEnumerable, IEnumerator<LinkedListNode<LFUCache<TKey, TValue>.CacheNode>>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private LinkedListNode<LFUCache<TKey, TValue>.CacheNode> <>2__current;
            public LFUCache<TKey, TValue> <>4__this;
            private int <>l__initialThreadId;
            public LinkedListNode<LFUCache<TKey, TValue>.CacheNode> <node>5__5;

            [DebuggerHidden]
            public <get_Nodes>d__4(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        this.<node>5__5 = this.<>4__this.lfuList.First;
                        while (this.<node>5__5 != null)
                        {
                            this.<>2__current = this.<node>5__5;
                            this.<>1__state = 1;
                            return true;
                        Label_0055:
                            this.<>1__state = -1;
                            this.<node>5__5 = this.<node>5__5.Next;
                        }
                        break;

                    case 1:
                        goto Label_0055;
                }
                return false;
            }

            [DebuggerHidden]
            IEnumerator<LinkedListNode<LFUCache<TKey, TValue>.CacheNode>> IEnumerable<LinkedListNode<LFUCache<TKey, TValue>.CacheNode>>.GetEnumerator()
            {
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    return (LFUCache<TKey, TValue>.<get_Nodes>d__4) this;
                }
                return new LFUCache<TKey, TValue>.<get_Nodes>d__4(0) { <>4__this = this.<>4__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Collections.Generic.LinkedListNode<EgoalTech.Commons.Cache.LFUCache<TKey,TValue>.CacheNode>>.GetEnumerator()

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
            }

            LinkedListNode<LFUCache<TKey, TValue>.CacheNode> IEnumerator<LinkedListNode<LFUCache<TKey, TValue>.CacheNode>>.Current =>
                this.<>2__current

            object IEnumerator.Current =>
                this.<>2__current
        }

        [CompilerGenerated]
        private sealed class <IterateFrom>d__8 : IEnumerable<LinkedListNode<LFUCache<TKey, TValue>.CacheNode>>, IEnumerable, IEnumerator<LinkedListNode<LFUCache<TKey, TValue>.CacheNode>>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private LinkedListNode<LFUCache<TKey, TValue>.CacheNode> <>2__current;
            public LinkedListNode<LFUCache<TKey, TValue>.CacheNode> <>3__node;
            public LFUCache<TKey, TValue> <>4__this;
            private int <>l__initialThreadId;
            public LinkedListNode<LFUCache<TKey, TValue>.CacheNode> node;

            [DebuggerHidden]
            public <IterateFrom>d__8(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        while (this.node != null)
                        {
                            this.<>2__current = this.node;
                            this.<>1__state = 1;
                            return true;
                        Label_003F:
                            this.<>1__state = -1;
                            this.node = this.node.Next;
                        }
                        break;

                    case 1:
                        goto Label_003F;
                }
                return false;
            }

            [DebuggerHidden]
            IEnumerator<LinkedListNode<LFUCache<TKey, TValue>.CacheNode>> IEnumerable<LinkedListNode<LFUCache<TKey, TValue>.CacheNode>>.GetEnumerator()
            {
                LFUCache<TKey, TValue>.<IterateFrom>d__8 d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (LFUCache<TKey, TValue>.<IterateFrom>d__8) this;
                }
                else
                {
                    d__ = new LFUCache<TKey, TValue>.<IterateFrom>d__8(0) {
                        <>4__this = this.<>4__this
                    };
                }
                d__.node = this.<>3__node;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Collections.Generic.LinkedListNode<EgoalTech.Commons.Cache.LFUCache<TKey,TValue>.CacheNode>>.GetEnumerator()

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
            }

            LinkedListNode<LFUCache<TKey, TValue>.CacheNode> IEnumerator<LinkedListNode<LFUCache<TKey, TValue>.CacheNode>>.Current =>
                this.<>2__current

            object IEnumerator.Current =>
                this.<>2__current
        }

        private class CacheNode
        {
            public TValue Data { get; set; }

            public TKey Key { get; set; }

            public int UseCount { get; set; }
        }
    }
}

