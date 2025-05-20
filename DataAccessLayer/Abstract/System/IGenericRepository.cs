
using System.Linq.Expressions;

namespace DataAccessLayer.Abstract.System
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task DeleteAsync(T entity);
        Task UpdateAsync(T entity);
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task SaveChangesAsync();
    }
}
