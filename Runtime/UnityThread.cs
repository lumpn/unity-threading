//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using System.Collections;
using System.Collections.Generic;

namespace Lumpn.Threading
{
    internal sealed class UnityThread : IThread
    {
        private readonly Queue<Task> pendingTasks;
        public readonly string name;
        private bool started;
        private bool canceled;

        public bool isRunning { get { return started && !canceled; } }
        public bool isIdle { get { return pendingTasks.Count <= 0; } }
        public int queueLength { get { return pendingTasks.Count; } }
        public ISynchronizationContext context { get { return this; } }

        public UnityThread(string name, int initialCapacity)
        {
            this.pendingTasks = new Queue<Task>(initialCapacity);
            this.name = name;
            this.started = false;
            this.canceled = false;
        }

        public IEnumerator Start()
        {
            started = true;
            while (!canceled)
            {
                int numTasks;
                lock (pendingTasks)
                {
                    numTasks = pendingTasks.Count;
                }

                // explicitly only pump as many tasks as are queued right now
                // because executing tasks can enqueue more tasks
                Run(numTasks);

                yield return CoroutineUtils.waitForNextFrame;
            }
        }

        public void Stop()
        {
            lock (pendingTasks)
            {
                canceled = true;
            }
        }

        public void Post(Callback callback, object owner, object state)
        {
            var pendingTask = new Task(callback, owner, state);
            lock (pendingTasks)
            {
                pendingTasks.Enqueue(pendingTask);
            }
        }

        public override string ToString()
        {
            return name;
        }

        private bool TryDequeue(out Task task)
        {
            lock (pendingTasks)
            {
                if (pendingTasks.Count > 0)
                {
                    task = pendingTasks.Dequeue();
                    return true;
                }
            }

            task = default(Task);
            return false;
        }

        private void Run(int numTasks)
        {
            for (int i = 0; i < numTasks; i++)
            {
                if (!TryDequeue(out Task task)) return;
                task.Invoke();
            }
        }
    }
}
