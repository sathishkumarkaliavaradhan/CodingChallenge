using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodingChallenge.Tests
{
    [TestClass]
    public class LRUCacheEdgeCaseTests
    {
        [TestMethod]
        public void Methods_ThrowAfterDispose()
        {
            var cache = new LRUCache(2);
            cache.Dispose();
            Assert.ThrowsException<ObjectDisposedException>(() => cache.Put(1, "one"));
            Assert.ThrowsException<ObjectDisposedException>(() => cache.Get(1));
        }

        [TestMethod]
        public void PutGet_IntMinMaxKeys()
        {
            var cache = new LRUCache(2);
            cache.Put(int.MinValue, "min");
            cache.Put(int.MaxValue, "max");
            Assert.AreEqual("min", cache.Get(int.MinValue));
            Assert.AreEqual("max", cache.Get(int.MaxValue));
        }

        [TestMethod]
        public void PutGet_LargeStringValues()
        {
            var cache = new LRUCache(2);
            string large = new string('x', 100_000);
            cache.Put(1, large);
            Assert.AreEqual(100_000, cache.Get(1).Length);
        }

        [TestMethod]
        public void RapidRepeatedPutGet_SameKey()
        {
            var cache = new LRUCache(1);
            for (int i = 0; i < 1000; i++)
            {
                cache.Put(1, $"val{i}");
                Assert.AreEqual($"val{i}", cache.Get(1));
            }
        }
    }
}
