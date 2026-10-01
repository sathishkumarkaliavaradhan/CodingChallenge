namespace CodingChallenge
{
    /// <summary>
    /// Represents a single entry stored in the LRU cache.
    /// </summary>
    public class CacheItem
    {
        /// <summary>
        /// Gets or sets the integer key for the cache entry.
        /// </summary>
        public int Key { get; set; }

        /// <summary>
        /// Gets or sets the string value for the cache entry. May be null to represent an explicit null value.
        /// </summary>
        public string Value { get; set; }
    }
}
