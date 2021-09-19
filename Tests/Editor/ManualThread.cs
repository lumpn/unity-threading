//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using System.Collections.Generic;

namespace Lumpn.Threading.Tests
{
    public sealed class ManualThread : IThread
    {
        private readonly Queue<Task> tasks = new Queue<Task>();

        public bool IsRunning { get { return true; } }
        public bool IsIdle { get { return QueueLength < 1; } }
        public int QueueLength { get { return tasks.Count; } }
        public ISynchronizationContext Context { get { return this; } }

        public void Post(Callback callback, object owner, object state)
        {
            var task = new Task(callback, owner, state);
            tasks.Enqueue(task);
        }

        public void Stop() { }

        public bool Step()
        {
            if (tasks.Count < 1) return false;

            var task = tasks.Dequeue();
            task.Invoke();
            return true;
        }
    }
}
