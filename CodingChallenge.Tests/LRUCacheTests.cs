using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodingChallenge.Tests
{
    [TestClass]
    public class LRUCacheTests
    {
        [DataTestMethod]
        [DataRow(2, 1, "one", 2, "two")]
        [DataRow(3, 10, "ten", 20, "twenty")]
        public void BasicPutGet_Works(int capacity, int key1, string value1, int key2, string value2)
        {
            var cache = new LRUCache(capacity);
            cache.Put(key1, value1);
            cache.Put(key2, value2);
            Assert.AreEqual(value1, cache.Get(key1));
            Assert.AreEqual(value2, cache.Get(key2));
        }

        [TestMethod]
        public void Get_NonExistent_ReturnsEmpty()
        {
            var cache = new LRUCache(2);
            Assert.AreEqual(string.Empty, cache.Get(99));
        }

        [TestMethod]
        public void LRU_Eviction_Works()
        {
            var cache = new LRUCache(2);
            cache.Put(1, "one");
            cache.Put(2, "two");
            cache.Get(1); // 1 is most recent
            cache.Put(3, "three"); // should evict 2
            Assert.AreEqual("one", cache.Get(1));
            Assert.AreEqual(string.Empty, cache.Get(2));
            Assert.AreEqual("three", cache.Get(3));
        }

        [TestMethod]
        public void UpdateValue_MovesToRecent()
        {
            var cache = new LRUCache(2);
            cache.Put(1, "one");
            cache.Put(2, "two");
            cache.Put(1, "uno"); // update 1
            cache.Put(3, "three"); // should evict 2
            Assert.AreEqual("uno", cache.Get(1));
            Assert.AreEqual(string.Empty, cache.Get(2));
            Assert.AreEqual("three", cache.Get(3));
        }
    }
}
