//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using NUnit.Framework;

namespace Lumpn.Threading
{
    [TestFixture]
    public sealed class ObjectPoolTest
    {
        [Test]
        public void TestPutSingle()
        {
            var pool = new ObjectPool<object>(4);
            var obj = new object();
            bool putResult = pool.TryPut(obj);
            Assert.IsTrue(putResult);
            Assert.AreEqual(1, pool.Count);

            bool getResult = pool.TryGet(out object obj2);
            Assert.IsTrue(getResult);
            Assert.AreSame(obj, obj2);
        }

        [Test]
        public void TestPutMultiple()
        {
            var pool = new ObjectPool<object>(4);

            for (int i = 0; i < 4; i++)
            {
                var putResult = pool.TryPut(new object());
                Assert.IsTrue(putResult);
            }
            Assert.AreEqual(4, pool.Count);

            for (int i = 0; i < 4; i++)
            {
                var getResult = pool.TryGet(out object obj);
                Assert.IsTrue(getResult);
                Assert.IsNotNull(obj);
            }
            Assert.AreEqual(0, pool.Count);

            {
                var getResult = pool.TryGet(out object obj);
                Assert.IsFalse(getResult);
                Assert.IsNull(obj);
            }
        }
    }
}
