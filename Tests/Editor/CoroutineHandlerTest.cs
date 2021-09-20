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
        public void TestAdvanceCoroutine()
        {
            Assert.AreEqual(0, stepCounter);

            var thread = new ManualThread();
            var coroutine = thread.StartCoroutine(Coroutine(), null);

            Assert.AreEqual(0, stepCounter);
            Assert.IsTrue(coroutine.keepWaiting);

            bool stepResult;
            stepResult = thread.Step();
            Assert.IsTrue(stepResult);
            Assert.AreEqual(1, stepCounter);
            Assert.IsTrue(coroutine.keepWaiting);

            stepResult = thread.Step();
            Assert.IsTrue(stepResult);
            Assert.AreEqual(2, stepCounter);
            Assert.IsTrue(coroutine.keepWaiting);

            stepResult = thread.Step();
            Assert.IsTrue(stepResult);
            Assert.AreEqual(3, stepCounter);
            Assert.IsFalse(coroutine.keepWaiting);

            stepResult = thread.Step();
            Assert.IsTrue(stepResult);
            Assert.AreEqual(3, stepCounter);
            Assert.IsFalse(coroutine.keepWaiting);

            stepResult = thread.Step();
            Assert.IsFalse(stepResult);
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
