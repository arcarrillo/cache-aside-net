using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace cacheaside.library.interfaces;
public interface IReaderRepository<T> where T : class
{
    /// <summary>
    /// Get all items of type T
    /// </summary>
    /// <returns>Collection of items of type T</returns>
    Task<List<T>> GetAll();
    /// <summary>
    /// Get items of type T filtered by a Lambda expression
    /// </summary>
    /// <param name="expression">Filter to apply</param>
    /// <returns>Collection of items of type T that matches with the filter specification</returns>
    Task<List<T>> GetAll(Expression<Func<T, bool>> expression);
    /// <summary>
    /// Get first found item filtered by an expression
    /// </summary>
    /// <param name="expression">Lambda filter expression</param>
    /// <returns>First object found</returns>
    Task<T?> GetOne(Expression<Func<T, bool>> expression);
    /// <summary>
    /// Asks if there is any item filtered by a Lambda expression
    /// </summary>
    /// <param name="expression">Lambda filter expression</param>
    /// <returns>Return true if it finds any, otherwise false</returns>
    Task<bool> Any(Expression<Func<T, bool>> expression);
}