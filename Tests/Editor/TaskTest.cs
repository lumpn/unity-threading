//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using NUnit.Framework;

namespace Lumpn.Threading.Tests
{
    [TestFixture]
    public sealed class TaskTest
    {
        private int counter = 0;

        [Test]
        public void Invoke()
        {
            var task = new Task(IncrementCounter, this, null);
            Assert.AreEqual(0, counter);

            task.Invoke();
            Assert.AreEqual(1, counter);

            task.Invoke();
            Assert.AreEqual(2, counter);
        }

        private static void IncrementCounter(object owner, object state)
        {
            var test = (TaskTest)owner;
            test.counter++;
        }
    }
}
