//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Lumpn
{
    [TestFixture]
    public class CoroutineHandlerTest
    {
        private sealed class ManualThread : IThread
        {
            private readonly Queue<Task> tasks = new Queue<Task>();

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
                task.Execute();
                return true;
            }
        }

        private int stepCounter = 0;

        [Test]
        public void TestAdvanceCoroutine()
        {
            Assert.AreEqual(0, stepCounter);

            var thread = new ManualThread();
            var wait = thread.StartCoroutine(Coroutine());

            Assert.AreEqual(0, stepCounter);
            Assert.IsTrue(wait.keepWaiting);

            bool stepResult;
            stepResult = thread.Step();
            Assert.IsTrue(stepResult);
            Assert.AreEqual(1, stepCounter);
            Assert.IsTrue(wait.keepWaiting);

            stepResult = thread.Step();
            Assert.IsTrue(stepResult);
            Assert.AreEqual(2, stepCounter);
            Assert.IsTrue(wait.keepWaiting);

            stepResult = thread.Step();
            Assert.IsTrue(stepResult);
            Assert.AreEqual(3, stepCounter);
            Assert.IsFalse(wait.keepWaiting);

            stepResult = thread.Step();
            Assert.IsTrue(stepResult);
            Assert.AreEqual(3, stepCounter);
            Assert.IsFalse(wait.keepWaiting);

            stepResult = thread.Step();
            Assert.IsFalse(stepResult);
            Assert.AreEqual(3, stepCounter);
            Assert.IsFalse(wait.keepWaiting);
        }

        private IEnumerator Coroutine()
        {
            stepCounter = 1;
            yield return null;
            stepCounter = 2;
            yield return CoroutineUtils.waitForNextFrame;
            stepCounter = 3;
        }
    }
}
