//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using System.Collections.Generic;
using NUnit.Framework;

namespace Lumpn.Threading.Tests
{
    [TestFixture]
    public sealed class StackTest
    {
        [Test]
        public void PushSingle()
        {
            var pool = new Stack<object>(4);
            var obj = new object();
            pool.Push(obj);
            Assert.AreEqual(1, pool.Count);

            bool success = pool.TryPop(out object obj2);
            Assert.IsTrue(success);
            Assert.AreSame(obj, obj2);
        }

        [Test]
        public void PushMultiple()
        {
            var pool = new Stack<object>(4);

            for (int i = 0; i < 4; i++)
            {
                pool.Push(new object());
            }
            Assert.AreEqual(4, pool.Count);

            for (int i = 0; i < 4; i++)
            {
                var success = pool.TryPop(out object obj);
                Assert.IsTrue(success);
                Assert.IsNotNull(obj);
            }
            Assert.AreEqual(0, pool.Count);
        }

        [Test]
        public void PopEmpty()
        {
            var pool = new Stack<object>(4);

            var success = pool.TryPop(out object obj);
            Assert.IsFalse(success);
            Assert.IsNull(obj);
        }
    }
}
