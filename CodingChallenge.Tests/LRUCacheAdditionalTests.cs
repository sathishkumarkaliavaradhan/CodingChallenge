using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodingChallenge.Tests
{
    [TestClass]
    public class LRUCacheAdditionalTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void ZeroCapacity_ThrowsOrInvalid()
        {
            var cache = new LRUCache(0);
            cache.Put(1, "one");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void NegativeCapacity_ThrowsOrInvalid()
        {
            var cache = new LRUCache(-1);
            cache.Put(1, "one");
        }

        [TestMethod]
        public void OverwriteExistingKey_UpdatesValueAndOrder()
        {
            var cache = new LRUCache(2);
            cache.Put(1, "one");
            cache.Put(2, "two");
            cache.Put(1, "uno"); // update value
            cache.Put(3, "three"); // should evict 2
            Assert.AreEqual("uno", cache.Get(1));
            Assert.AreEqual(string.Empty, cache.Get(2));
            Assert.AreEqual("three", cache.Get(3));
        }

        [TestMethod]
        public void AllKeysEvicted_NoneRemain()
        {
            var cache = new LRUCache(2);
            cache.Put(1, "one");
            cache.Put(2, "two");
            cache.Put(3, "three");
            cache.Put(4, "four");
            Assert.AreEqual(string.Empty, cache.Get(1));
            Assert.AreEqual(string.Empty, cache.Get(2));
            Assert.AreEqual("three", cache.Get(3));
            Assert.AreEqual("four", cache.Get(4));
        }

        [TestMethod]
        public void NullOrEmptyValues_StoredAndRetrieved()
        {
            var cache = new LRUCache(2);
            cache.Put(1, null);
            cache.Put(2, "");
            Assert.IsNull(cache.Get(1));
            Assert.AreEqual(string.Empty, cache.Get(2));
        }       
    }
}
