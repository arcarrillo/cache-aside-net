using cacheaside.library.interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace cacheaside.library.implementations.ef
{
    public class EFReaderRepository<T>(DbContext context) : IReaderRepository<T> where T : class
    {
        public virtual async Task<bool> Any(Expression<Func<T, bool>> expression) => (await GetAll(expression))?.Any() ?? false;

        public virtual Task<List<T>> GetAll() => context.Set<T>().ToListAsync();

        public virtual Task<List<T>> GetAll(Expression<Func<T, bool>> expression) => context.Set<T>().Where(expression).ToListAsync();

        public virtual Task<T?> GetOne(Expression<Func<T, bool>> expression) => context.Set<T>().FirstOrDefaultAsync(expression);
    }
}
