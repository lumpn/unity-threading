//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using System.Collections.Generic;

namespace Lumpn.Threading
{
    internal sealed class Pool<T> where T : new()
    {
        private readonly Stack<T> pool;

        public Pool(int initialSize)
        {
            this.pool = new Stack<T>(initialSize);
        }

        public T Get()
        {
            lock (pool)
            {
                return pool.PopOrNew();
            }
        }

        public void Return(T obj)
        {
            lock (pool)
            {
                pool.Push(obj);
            }
        }
    }
}
