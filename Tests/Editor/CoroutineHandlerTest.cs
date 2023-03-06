//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using System.Collections;
using NUnit.Framework;

namespace Lumpn.Threading.Tests
{
    [TestFixture]
    public sealed class CoroutineHandlerTest
    {
        private int stepCounter = 0;

        [Test]
        public void AdvanceCoroutine()
        {
            Assert.AreEqual(0, stepCounter);

            var thread = new ManualThread();
            var coroutine = thread.StartCoroutine(Coroutine(), null);

            Assert.AreEqual(0, stepCounter);
            Assert.IsTrue(coroutine.keepWaiting);

            bool success;
            success = thread.Step();
            Assert.IsTrue(success);
            Assert.AreEqual(1, stepCounter);
            Assert.IsTrue(coroutine.keepWaiting);

            success = thread.Step();
            Assert.IsTrue(success);
            Assert.AreEqual(2, stepCounter);
            Assert.IsTrue(coroutine.keepWaiting);

            success = thread.Step();
            Assert.IsTrue(success);
            Assert.AreEqual(3, stepCounter);
            Assert.IsFalse(coroutine.keepWaiting);

            success = thread.Step();
            Assert.IsTrue(success);
            Assert.AreEqual(3, stepCounter);
            Assert.IsFalse(coroutine.keepWaiting);

            success = thread.Step();
            Assert.IsFalse(success);
            Assert.AreEqual(3, stepCounter);
            Assert.IsFalse(coroutine.keepWaiting);
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
