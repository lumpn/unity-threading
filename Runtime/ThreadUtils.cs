//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using UnityEngine;
using ThreadPriority = System.Threading.ThreadPriority;

namespace Lumpn
{
    public static class ThreadUtils
    {
        public static WorkerThread StartWorkerThread(string group, string name, ThreadPriority priority, int initialCapacity)
        {
            var thread = new WorkerThread(group, name, priority, initialCapacity);
            thread.Start();
            return thread;
        }

        public static UnityThread StartUnityThread(string name, int initialCapacity, MonoBehaviour host)
        {
            var thread = new UnityThread(name, initialCapacity);
            host.StartCoroutine(thread.Run());
            return thread;
        }
    }
}
