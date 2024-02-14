using System;

namespace cacheaside.library.interfaces;

public interface IWriterRepository<T> where T : class
{
    /// <summary>
    /// Add item to the storage
    /// </summary>
    /// <param name="item">Item to be added</param>
    void Add(T item);
    /// <summary>
    /// Remove an item placed at the remote storage
    /// </summary>
    /// <param name="item">Item to remove</param>
    void Remove(T item);
    /// <summary>
    /// Updates an item placed at the remote storage
    /// </summary>
    /// <param name="item">Item to remove</param>
    void Update(T item);
    /// <summary>
    /// Gets a transaction where the actions are going to be executed
    /// </summary>
    /// <returns>Returns an object fo type ITransaction</returns>
    ITransaction BeginTransaction(Action? transactionCommited = null);
}