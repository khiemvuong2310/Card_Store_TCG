using System.Linq.Expressions;

namespace CardStore.Data.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class
{
    // Query methods
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
    
    // Paging methods
    Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> predicate);
    
    // Command methods
    Task<TEntity> AddAsync(TEntity entity);
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities);
    
    // Unit of work
    Task<int> SaveChangesAsync();
}