//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using UnityEngine;

namespace Lumpn
{
    public static class ThreadUtils
    {
        public static UnityThread StartUnityThread(string name, int initialCapacity, MonoBehaviour host)
        {
            var thread = new UnityThread(name, initialCapacity);
            host.StartCoroutine(thread.Run());
            return thread;
        }
    }
}
