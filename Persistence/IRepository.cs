namespace Persistence;

public interface IRepository<T> : IQueryable<T>, IAsyncEnumerable<T>
    where T : class 
{
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Add(T objectToAdd);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}