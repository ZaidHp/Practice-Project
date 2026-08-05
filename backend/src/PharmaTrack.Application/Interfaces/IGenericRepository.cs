using System.Linq.Expressions;
using PharmaTrack.Domain.Common;

namespace PharmaTrack.Application.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void SoftDelete(T entity);
    Task<int> SaveChangesAsync();
    Task<IEnumerable<T>> FindIgnoreQueryFiltersAsync(Expression<Func<T, bool>> predicate);
}