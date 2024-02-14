using System;
using System.Threading.Tasks;

namespace cacheaside.library.interfaces;

public interface ICacheProvider
{
    /// <summary>
    /// Generic method to get an item from the cache given a key.
    /// The result should be serialized to the type T.
    /// </summary>
    /// <typeparam name="T">Type of expected result</typeparam>
    /// <param name="key">Given key</param>
    /// <returns>Returns an object of type T or null</returns>
    Task<T?> GetValueAsync<T>(string key) where T : class;
    /// <summary>
    /// Sets an object on the cache.
    /// This method is generic to store any object type.
    /// </summary>
    /// <typeparam name="T">Type of object to save</typeparam>
    /// <param name="key">Given key</param>
    /// <param name="value">Object to save</param>
    /// <param name="duration">Timespan that represents the duration of the object in the cache</param>
    Task<bool> SetValueAsync<T>(string key, T value, TimeSpan duration) where T : class;
    /// <summary>
    /// Method that gets or initialize a cache item with a given key.
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="key">Given key</param>
    /// <param name="functionToObtain">Function to obtain a valid value in case of null</param>
    /// <param name="duration">Timespan that represents the duration of the object in the cache</param>
    /// <returns>Returns a string object</returns>
    Task<T?> GetValueOrInitializeAsync<T>(string key, Func<Task<T>> functionToObtain, TimeSpan duration) where T : class;
    /// <summary>
    /// Removes items from the cache by a given pattern
    /// </summary>
    /// <param name="key">Given key</param>
    /// <returns>Returns true if success, false if not</returns>
    bool RemoveByPattern(string pattern);
    /// <summary>
    /// Check if an item exists given its key.
    /// </summary>
    /// <param name="key">Key to be checked</param>
    /// <returns>Returns a boolean that indicates if exists or not</returns>
    Task<bool> Exists(string key);
}