using cacheaside.library.interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace cacheaside.library.implementations.ef;

public class EFTransaction(IDbContextTransaction transaction, Action commitedCallback) : ITransaction, IDisposable
{

    public bool Completed { get; private set; } = false;

    public void Commit()
    {
        transaction.Commit();
        Completed = true;
        commitedCallback?.Invoke();
    }

    public void Dispose()
    {
        if (!Completed) 
            Rollback();
    }

    public void Rollback()
    {
        transaction.Rollback();
        Completed = true;
    }
}
