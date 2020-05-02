//----------------------------------------
// MIT License
// Copyright(c) 2019 Jonas Boetel
//----------------------------------------
using NUnit.Framework;

namespace Lumpn
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

            object obj2;
            bool getResult = pool.TryGet(out obj2);
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
                object obj;
                var getResult = pool.TryGet(out obj);
                Assert.IsTrue(getResult);
                Assert.IsNotNull(obj);
            }
            Assert.AreEqual(0, pool.Count);

            {
                object obj;
                var getResult = pool.TryGet(out obj);
                Assert.IsFalse(getResult);
                Assert.IsNull(obj);
            }
        }
    }
}
