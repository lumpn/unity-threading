//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using System.Threading;
using NUnit.Framework;

namespace Lumpn.Threading.Tests
{
    [TestFixture]
    public sealed class WorkerThreadTest
    {
        private int counter = 0;

        [Test]
        public void Post()
        {
            var thread = new WorkerThread("Test", "TestPost", ThreadPriority.BelowNormal, 10);
            Assert.IsFalse(thread.isIdle);
            Assert.AreEqual(0, thread.queueLength);
            Assert.AreEqual(0, counter);

            thread.Post(IncrementCounter, this, null);
            thread.Post(IncrementCounter, this, null);
            thread.Post(IncrementCounter, this, null);
            Assert.IsFalse(thread.isIdle);
            Assert.AreEqual(3, thread.queueLength);
            Assert.AreEqual(0, counter);

            thread.Start();
            while (!thread.isIdle) Thread.Sleep(1);

            Assert.IsTrue(thread.isIdle);
            Assert.AreEqual(0, thread.queueLength);
            Assert.AreEqual(3, counter);

            thread.Stop();
        }

        private static void IncrementCounter(object owner, object state)
        {
            var test = (WorkerThreadTest)owner;
            test.counter++;
        }
    }
}
