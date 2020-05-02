//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using System.Collections;
using System.Collections.Generic;

namespace Lumpn.Threading
{
    public sealed class UnityThread : IThread
    {
        private readonly Queue<Task> tasks;
        public readonly string name;
        private bool canceled;

        public bool IsIdle { get { return QueueLength <= 0; } }
        public int QueueLength { get { return tasks.Count; } }
        public ISynchronizationContext Context { get { return this; } }

        public UnityThread(string name, int initialCapacity)
        {
            this.tasks = new Queue<Task>(initialCapacity);
            this.name = name;
            this.canceled = false;
        }

        private void Run(int numTasks)
        {
            for (int i = 0; i < numTasks; i++)
            {
                Task task;
                if (!TryDequeue(out task)) return;
                task.Invoke();
            }
        }

        public IEnumerator Run()
        {
            while (!canceled)
            {
                int num;
                lock (tasks)
                {
                    num = tasks.Count;
                }

                // explicitly only pump as many tasks as are queued right now
                // because executing tasks can enqueue more tasks
                Run(num);

                yield return CoroutineUtils.waitForNextFrame;
            }
        }

        public void Post(Callback callback, object owner, object state)
        {
            var pendingTask = new Task(callback, owner, state);
            lock (tasks)
            {
                tasks.Enqueue(pendingTask);
            }
        }

        private bool TryDequeue(out Task task)
        {
            lock (tasks)
            {
                if (tasks.Count > 0)
                {
                    task = tasks.Dequeue();
                    return true;
                }
            }

            task = default(Task);
            return false;
        }

        public void Stop()
        {
            lock (tasks)
            {
                canceled = true;
            }
        }

        public override string ToString()
        {
            return name;
        }
    }
}
