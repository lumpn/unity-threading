//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using NUnit.Framework;

namespace Lumpn
{
    [TestFixture]
    public sealed class UnityThreadTest
    {
        private int counter = 0;

        [Test]
        public void TestPost()
        {
            var thread = new UnityThread("Test", 10);
            Assert.IsTrue(thread.IsIdle);
            Assert.AreEqual(0, thread.QueueLength);
            Assert.AreEqual(0, counter);

            thread.Post(IncrementCounter, this, null);
            thread.Post(IncrementCounter, this, null);
            thread.Post(IncrementCounter, this, null);
            Assert.IsFalse(thread.IsIdle);
            Assert.AreEqual(3, thread.QueueLength);
            Assert.AreEqual(0, counter);

            var pump = thread.Run();
            while (!thread.IsIdle) pump.MoveNext();

            Assert.IsTrue(thread.IsIdle);
            Assert.AreEqual(0, thread.QueueLength);
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
