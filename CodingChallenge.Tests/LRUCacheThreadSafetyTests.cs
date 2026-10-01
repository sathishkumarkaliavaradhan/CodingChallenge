using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodingChallenge.Tests
{
    [TestClass]
    public class LRUCacheThreadSafetyTests
    {
        [TestMethod]
        public void LRUCache_ThreadSafety_UnderConcurrentAccess()
        {
            var cache = new LRUCache(5);
            int numTasks = 10;
            int numOpsPerTask = 1000;
            var tasks = new List<Task>();

            for (int t = 0; t < numTasks; t++)
            {
                int taskNum = t;
                tasks.Add(Task.Run(() =>
                {
                    for (int i = 0; i < numOpsPerTask; i++)
                    {
                        int key = (taskNum + i) % 10;
                        cache.Put(key, $"val_{taskNum}_{i}");
                        var _ = cache.Get(key);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // After all operations, cache should not throw and should have at most 5 items
            int count = 0;
            for (int k = 0; k < 10; k++)
            {
                var v = cache.Get(k);
                if (!string.IsNullOrEmpty(v)) count++;
            }
            Assert.IsTrue(count <= 5, $"Cache contains more than 5 items: {count}");
        }

        [TestMethod]
        public void ThreadSafety_HeavyRandomLoad()
        {
            var cache = new LRUCache(10);
            int numTasks = 20;
            int numOpsPerTask = 2000;
            var tasks = new List<Task>();
            var rand = new Random();

            for (int t = 0; t < numTasks; t++)
            {
                tasks.Add(Task.Run(() =>
                {
                    for (int i = 0; i < numOpsPerTask; i++)
                    {
                        int key = rand.Next(0, 20);
                        cache.Put(key, $"val_{key}_{i}");
                        var _ = cache.Get(key);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
            // Should not throw and should have at most 10 items
            int count = 0;
            for (int k = 0; k < 20; k++)
            {
                var v = cache.Get(k);
                if (!string.IsNullOrEmpty(v)) count++;
            }
            Assert.IsTrue(count <= 10, $"Cache contains more than 10 items: {count}");
        }
    }
}
