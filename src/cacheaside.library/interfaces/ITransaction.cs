using System;

namespace cacheaside.library.interfaces;

public interface ITransaction : IDisposable
{
    /// <summary>
    /// Commits all pending operations inside the remote engine storage
    /// </summary>
    void Commit();
    /// <summary>
    /// Rollbacks all pendant operations.
    /// </summary>
    void Rollback();
}
