using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class Repository<T> : IRepository<T> where T : class
{
    #region Fields
    
    private readonly ApplicationDbContext context;
    private readonly DbSet<T> dbSet;
    protected readonly IQueryable<T> SourceQuery;
    
    #endregion Fields
    
    #region Constructor

    public Repository(ApplicationDbContext context)
    {
        this.context = context;
        dbSet = context.Set<T>();
        SourceQuery = context.Set<T>();
    }
    
    #endregion Constructor
    
    #region Methods
    
    public async Task<T> GetByIdAsync(Guid id)
    {
        return await dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await dbSet.AddAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {

        dbSet.Update(entity);
    }

    public async Task DeleteAsync(T entity)
    {
        dbSet.Remove(entity);
    }
    
    #endregion Methods

    public IEnumerator<T> GetEnumerator()
    {
        return SourceQuery.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Type ElementType => SourceQuery.ElementType;
    public Expression Expression => SourceQuery.Expression;
    public IQueryProvider Provider  => SourceQuery.Provider;
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
    {
        return ((IAsyncEnumerable<T>)SourceQuery).GetAsyncEnumerator(cancellationToken);
    }
}