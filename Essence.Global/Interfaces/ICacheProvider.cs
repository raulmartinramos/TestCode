using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Essence.Global
{
    public enum CacheItemPriority
    {
        Normal,
        High
    }

    /// <summary>
    /// Interface for caching providers - all providers should support expiring of caching items, all data add must be a reference type.
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Occurs when [key removed].
        /// </summary>
        event EventHandler KeyRemoved;

        /// <summary>
        /// Add a value to the cache with a relative expiry time, e.g 10 minutes.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="slidingExpiry">The sliding time when the key value pair should expire and be purged from the cache.</param>
        /// <param name="priority">Normal priority should be purged on low memory warning.</param>
        void Add<TKey, TValue>(TKey key, TValue value, TimeSpan slidingExpiry, CacheItemPriority priority = CacheItemPriority.Normal) where TValue : class;

        /// <summary>
        /// Add a value to the cache with an absolute time, e.g. 01/01/2020.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="absoluteExpiry">The absolute date time when the cache should expire and be purged the value.</param>
        /// <param name="priority">Normal priority will be should on low memory warning.</param>
        void Add<TKey, TValue>(TKey key, TValue value, DateTime absoluteExpiry, CacheItemPriority priority = CacheItemPriority.Normal) where TValue : class;

        /// <summary>
        /// Gets a value from the cache for specified key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>If the key exists in the cache then the value is returned, if the key does not exist then null is returned.</returns>
        TValue Get<TKey, TValue>(TKey key) where TValue : class;

        /// <summary>
        /// Remove a value from the cache for specified key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="key">The key.</param>
        void Remove<TKey>(TKey key);

        /// <summary>
        /// Clears the contents of the cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets an enumerator for keys of a specific type.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <returns>Returns an enumerator for keys of a specific type.</returns>
        IEnumerable<TKey> Keys<TKey>();

        /// <summary>
        /// Gets an enumerator for all the keys
        /// </summary>
        /// <returns>Returns an enumerator for all the keys.</returns>
        IEnumerable<object> Keys();

        /// <summary>
        /// Gets the total count of items in cache
        /// </summary>
        /// <returns>-1 if failed</returns>
        int TotalItems();

        /// <summary>
        /// Purges all cache item with normal priorities.
        /// </summary>
        /// <returns>Number of items removed (-1 if failed)</returns>
        int PurgeNormalPriorities();
    }
}
