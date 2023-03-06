//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using NUnit.Framework;

namespace Lumpn.Threading.Tests
{
    [TestFixture]
    public sealed class UnityThreadTest
    {
        private int counter = 0;

        [Test]
        public void Post()
        {
            var thread = new UnityThread("Test", 10);
            Assert.IsTrue(thread.isIdle);
            Assert.AreEqual(0, thread.queueLength);
            Assert.AreEqual(0, counter);

            thread.Post(IncrementCounter, this, null);
            thread.Post(IncrementCounter, this, null);
            thread.Post(IncrementCounter, this, null);
            Assert.IsFalse(thread.isIdle);
            Assert.AreEqual(3, thread.queueLength);
            Assert.AreEqual(0, counter);

            var pump = thread.Start();
            while (!thread.isIdle) pump.MoveNext();

            Assert.IsTrue(thread.isIdle);
            Assert.AreEqual(0, thread.queueLength);
            Assert.AreEqual(3, counter);

            thread.Stop();
        }

        private static void IncrementCounter(object owner, object state)
        {
            var test = (UnityThreadTest)owner;
            test.counter++;
        }
    }
}
