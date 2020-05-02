//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using System.Collections.Generic;

namespace Lumpn
{
    public sealed class ObjectPool<T> where T : class
    {
        private readonly Stack<T> pool;

        public int Count { get { return pool.Count; } }

        public ObjectPool(int initialCapacity)
        {
            pool = new Stack<T>(initialCapacity);
        }

        public bool TryGet(out T result)
        {
            if (pool.Count > 0)
            {
                result = pool.Pop();
                return true;
            }

            result = null;
            return false;
        }

        public bool TryPut(T obj)
        {
            pool.Push(obj);
            return true;
        }
    }
}
