//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using System.Collections.Generic;

namespace Lumpn.Threading.Tests
{
    public sealed class ManualThread : IThread
    {
        private readonly Queue<Task> pendingTasks = new Queue<Task>();

        public bool isRunning { get { return true; } }
        public bool isIdle { get { return pendingTasks.Count <= 0; } }
        public int queueLength { get { return pendingTasks.Count; } }
        public ISynchronizationContext context { get { return this; } }

        public void Post(Callback callback, object owner, object state)
        {
            var task = new Task(callback, owner, state);
            pendingTasks.Enqueue(task);
        }

        public void Stop() { }

        public bool Step()
        {
            if (pendingTasks.Count <= 0) return false;

            var task = pendingTasks.Dequeue();
            task.Invoke();
            return true;
        }
    }
}
