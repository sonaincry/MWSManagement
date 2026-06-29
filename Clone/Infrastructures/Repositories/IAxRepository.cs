using System.Linq.Expressions;

namespace Indotalent.Infrastructures.Repositories
{
    public interface IAxRepository<T> where T : class
    {
        IQueryable<T> Query();
        Task<List<T>> GetAllAsync();
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveChangesAsync();
    }
}