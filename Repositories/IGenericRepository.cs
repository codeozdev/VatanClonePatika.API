using System.Linq.Expressions;

namespace Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<bool> AnyAsync(int id);
    IQueryable<T> GetAll();
    ValueTask<T?> GetByIdAsync(int id);
    IQueryable<T> Where(Expression<Func<T, bool>> predicate);
    ValueTask AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}

