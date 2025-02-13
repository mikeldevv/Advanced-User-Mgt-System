namespace UMS.Contracts;

/// <summary>
///     Provides an interface for caching with basic Read, Write, and ReadAndDelete operations.
/// </summary>
public interface ICache
{
    /// <summary>
    ///     Asynchronously checks whether the specified key exists in the cache, and if so, when it will expire.
    /// </summary>
    /// <param name="key">The key to check for existence.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result is a tuple containing
    ///     a boolean indicating whether the key exists and a TimeSpan indicating how long until the key expires.
    /// </returns>
    Task<(bool Hit, TimeSpan? ExpiresIn)> Exists(string key);

    /// <summary>
    ///     Asynchronously reads the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to read.</param>
    /// <returns>
    ///     A task that represents the asynchronous read operation.
    ///     The task result contains the value associated with the specified key and its expiry.
    /// </returns>
    Task<(string Value, TimeSpan ExpiresIn)?> Read(string key);

    /// <summary>
    ///     Asynchronously removes the specified key from the cache.
    /// </summary>
    /// <param name="key">The key of the value to delete.</param>
    /// <returns>
    ///     A task that represents the asynchronous delete operation.
    ///     The task result contains the value associated with the specified key.
    /// </returns>
    Task Delete(string key);

    /// <summary>
    ///     Asynchronously writes the specified key and value to the cache.
    /// </summary>
    /// <param name="key">The key of the value to write.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="preserveTtlIfKeyIsExpiring">If the key already exists, preserve the lifespan.</param>
    /// <param name="ttl">
    ///     Optional parameter. Specifies the time duration after which the data should be deleted.
    ///     If null, the data does not expire.
    /// </param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    Task Write(string key, string value, bool preserveTtlIfKeyIsExpiring = false, TimeSpan? ttl = null);
}