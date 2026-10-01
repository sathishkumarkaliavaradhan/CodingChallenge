using System;
using System.Collections.Generic;
using System.Threading;

namespace CodingChallenge
{
    public class LRUCache : ICache, IDisposable
    {
        // Tracks whether Dispose has been called.
        private bool _disposed;
        private readonly int _capacity;
        private readonly Dictionary<int, LinkedListNode<CacheItem>> _cache;
        // _lruList keeps items ordered from most-recently-used (First) to least-recently-used (Last).
        private readonly LinkedList<CacheItem> _lruList = new();
        // Use a descriptive name for the reader/writer lock.
        private readonly ReaderWriterLockSlim _rwLock = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="LRUCache"/> class with the specified capacity.
        /// </summary>
        /// <param name="capacity">The maximum number of items the cache can hold.</param>
        /// <exception cref="ArgumentException">Thrown when capacity is less than 1.</exception>
        public LRUCache(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));
            }

            _capacity = capacity;
            _cache = new Dictionary<int, LinkedListNode<CacheItem>>(capacity);
        }

        /// <summary>
        /// Retrieves the value associated with the specified key from the cache.
        /// Moves the accessed item to the most recently used position.
        /// </summary>
        /// <param name="key">The key whose value should be retrieved.</param>
        /// <returns>The value associated with the key, or an empty string if the key does not exist.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the cache has been disposed.</exception>
        public string Get(int key)
        {
            // Use an upgradeable read lock: allows concurrent reads but enables promoting
            // to a write lock when we need to update the node ordering.
            _rwLock.EnterUpgradeableReadLock();
            try
            {
                if (!_cache.ContainsKey(key))
                {
                    // Design: this implementation returns string.Empty for a missing key
                    // (many unit tests assert this). If consumers need null to represent
                    // missing entries, adjust ICache contract and implementation.
                    return string.Empty;
                }

                var node = _cache[key];
                UpdateLRUNode(node);
                return node.Value.Value;
            }
            finally
            {
                _rwLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Inserts or updates the value for the specified key in the cache.
        /// Moves the item to the most recently used position.
        /// </summary>
        /// <param name="key">The key to insert or update.</param>
        /// <param name="value">The value to associate with the key.</param>
        /// <exception cref="ObjectDisposedException">Thrown if the cache has been disposed.</exception>
        public void Put(int key, string value)
        {
            // Delegate to internal method that acquires a write lock and performs
            // insert/update/eviction logic.
            UpdateCacheAndList(key, value);
        }

        /// <summary>
        /// Removes all items from the cache.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the cache has been disposed.</exception>
        public void Clear()
        {
            _rwLock.EnterWriteLock();
            try
            {
                _cache.Clear();
                _lruList.Clear();
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Releases all resources used by the cache.
        /// After calling Dispose, any use of Get/Put/Clear will throw <see cref="ObjectDisposedException"/>.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _rwLock?.Dispose();
        }

        private void UpdateLRUNode(LinkedListNode<CacheItem> node)
        {
            // Reorder the linked list to mark the node as most-recently-used.
            // This requires exclusive access because we mutate the linked list.
            _rwLock.EnterWriteLock();
            try
            {
                _lruList.Remove(node);
                _lruList.AddFirst(node);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        private void UpdateCacheAndList(int key, string value)
        {
            // All insert/update/eviction flows take the write lock to ensure
            // consistent state between the dictionary and the linked list.
            _rwLock.EnterWriteLock();
            try
            {
                if (_cache.ContainsKey(key))
                {
                    var node = _cache[key];
                    node.Value.Value = value;
                    // Updated an existing entry: move it to the MRU position.
                    _lruList.Remove(node);
                    _lruList.AddFirst(node);
                    return;
                }

                if (_cache.Count == _capacity)
                {
                    // Capacity reached: evict the least-recently-used (LRU) item,
                    // which is stored at the tail (Last) of the linked list.
                    var leastRecentlyUsed = _lruList.Last;
                    if (leastRecentlyUsed != null)
                    {
                        _cache.Remove(leastRecentlyUsed.Value.Key);
                        _lruList.RemoveLast();
                    }
                }

                var newItem = new CacheItem { Key = key, Value = value };
                var newNode = new LinkedListNode<CacheItem>(newItem);
                _cache[key] = newNode;
                // Newly added items are the most-recently-used.
                _lruList.AddFirst(newNode);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }
    }

}
