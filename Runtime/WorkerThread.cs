//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Profiling;

namespace Lumpn.Threading
{
    public sealed class WorkerThread : IThread
    {
        private readonly string group;
        private readonly string name;
        private readonly Thread thread;
        private readonly Queue<Task> pendingTasks;

        private bool started;
        private bool waiting;
        private bool canceled;

        public bool IsRunning { get { return started && !canceled; } }
        public bool IsIdle { get { return waiting && QueueLength <= 0; } }
        public int QueueLength { get { { return pendingTasks.Count; } } }
        public ISynchronizationContext Context { get { return this; } }

        public WorkerThread(string group, string name, ThreadPriority priority, int initialCapacity)
        {
            this.group = group;
            this.name = name;
            this.pendingTasks = new Queue<Task>(initialCapacity);
            this.thread = new Thread(ThreadMain)
            {
                Name = name,
                IsBackground = true,
                Priority = priority
            };
        }

        public void Start()
        {
            canceled = false;
            thread.Start(this);
        }

        public void Stop()
        {
            lock (pendingTasks)
            {
                canceled = true;
                Monitor.Pulse(pendingTasks);
            }
        }

        public void Post(Callback callback, object owner, object state)
        {
            lock (pendingTasks)
            {
                var pendingTask = new Task(callback, owner, state);
                pendingTasks.Enqueue(pendingTask);
                Monitor.Pulse(pendingTasks);
            }
        }

        private bool TryDequeue(out Task task)
        {
            lock (pendingTasks)
            {
                while (!canceled && pendingTasks.Count <= 0)
                {
                    waiting = true;
                    Monitor.Wait(pendingTasks);
                    waiting = false;
                }

                if (!canceled)
                {
                    task = pendingTasks.Dequeue();
                    return true;
                }
            }

            task = default(Task);
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", group, name);
        }

        private void Run()
        {
            Profiler.BeginThreadProfiling(group, name);

            started = true;
            while (TryDequeue(out Task task))
            {
                task.Invoke();
            }

            Profiler.EndThreadProfiling();
        }

        private static void ThreadMain(object state)
        {
            try
            {
                var worker = (WorkerThread)state;
                worker.Run();
            }
            catch (ThreadAbortException)
            {
                UnityEngine.Debug.LogFormat("Aborting thread '{0}'", state);
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }
    }
}
