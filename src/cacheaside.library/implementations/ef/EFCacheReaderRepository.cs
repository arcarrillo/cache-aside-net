using cacheaside.library.interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace cacheaside.library.implementations.ef;

public class EFCacheReaderRepository<T> : EFReaderRepository<T>, IRepositoryReader<T> where T : class
{
    ICacheProvider _cacheProvider;
    TimeSpan _dataTTL;

    public EFCacheReaderRepository(DbContext context, ICacheProvider cacheProvider, TimeSpan dataTTL) : base(context) {
        _cacheProvider = cacheProvider;
        _dataTTL = dataTTL;
    }

    public override async Task<bool> Any(Expression<Func<T, bool>> expression)
    {
        return (await GetAll(expression))?.Any() ?? false;
    }

    private string GetCacheName([CallerMemberName] string methodName = "", params string[] keys)
    {
        return $"{typeof(T).Name}:{methodName}:{String.Join(':', keys)}";
    }

    public override Task<List<T>> GetAll()
    {
        var cacheName = GetCacheName();
        return _cacheProvider.GetValueOrInitializeAsync<List<T>>(cacheName, () => base.GetAll(), _dataTTL)!;
    }

    public override Task<List<T>> GetAll(Expression<Func<T, bool>> expression)
    {
        var cacheName = GetCacheName(keys:expression.ToString());
        return _cacheProvider.GetValueOrInitializeAsync<List<T>>(
            cacheName, () => base.GetAll(expression), _dataTTL
        )!;
    }

    public override Task<T?> GetOne(Expression<Func<T, bool>> expression)
    {
        var cacheName = GetCacheName(keys: expression.ToString());
        return _cacheProvider.GetValueOrInitializeAsync<T>(
            cacheName,
            () => base.GetOne(expression),
            _dataTTL
        );
    }
}