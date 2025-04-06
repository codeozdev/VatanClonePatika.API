using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;


namespace Repositories
{
    public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext Context = context;
        private readonly DbSet<T> _dbSet = context.Set<T>();


        public Task<bool> AnyAsync(int id) => _dbSet.AnyAsync(x => x.Id.Equals(id)); // Filter icin ekledik

        public IQueryable<T> GetAll() => _dbSet.AsQueryable().AsNoTracking();

        public ValueTask<T?> GetByIdAsync(int id) => _dbSet.FindAsync(id);

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate).AsNoTracking();

        public async ValueTask AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Delete(T entity) => _dbSet.Remove(entity);
    }
}
