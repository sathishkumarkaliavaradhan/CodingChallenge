using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge
{
    public interface ICache
    {
        /// <summary>
        /// Retrieves the value associated with the specified key from the cache.
        /// </summary>
        /// <param name="key">The key whose value should be retrieved.</param>
        /// <returns>The value associated with the key, or null if the key does not exist.</returns>
        string Get(int key);

        /// <summary>
        /// Inserts or updates the value for the specified key in the cache.
        /// </summary>
        /// <param name="key">The key to insert or update.</param>
        /// <param name="value">The value to associate with the key.</param>
        void Put(int key, string value);

        /// <summary>
        /// Removes all items from the cache.
        /// </summary>
        void Clear();
    }
}
