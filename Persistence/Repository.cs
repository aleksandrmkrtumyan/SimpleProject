using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class Repository<T> : IRepository<T> where T : class
{
    #region Fields
    
    private readonly ApplicationDbContext context;
    private readonly DbSet<T> dbSet;
    
    #endregion Fields
    
    #region Constructor

    public Repository(ApplicationDbContext context)
    {
        this.context = context;
        dbSet = context.Set<T>();
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
}