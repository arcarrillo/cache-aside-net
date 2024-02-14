using cacheaside.library.interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace cacheaside.library.implementations.ef;

public class EFWriterRepository<T>(DbContext context) : IWriterRepository<T> where T : class
{
    protected EFTransaction? _currentTransaction;

    public virtual void Add(T item)
    {
        context.Add(item);
        context.SaveChanges();
    }

    public virtual ITransaction BeginTransaction(Action? transactionCommited = null)
    {
        _currentTransaction = new EFTransaction(
            context.Database.BeginTransaction(),
            () => transactionCommited?.Invoke()
        );
        return _currentTransaction;
    }

    public virtual void Remove(T item)
    {
        context.Remove(item);
        context.SaveChanges();
    }

    public virtual void Update(T item)
    {
        context.Update(item);
        context.SaveChanges();
    }

}
