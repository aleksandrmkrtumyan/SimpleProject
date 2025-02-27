using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class ChangesSaver<TDbContext> :IChangesSaver where TDbContext : DbContext
{
    #region Fields
    
    private readonly TDbContext context;
    
    #endregion Fields
    
    #region Constructor

    public ChangesSaver(TDbContext context)
    {
        this.context = context;
    }
    
    #endregion Constructor
    
    #region Methods
    
    public void SaveChanges()
    {
        context.SaveChanges();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
    
    #endregion Method
}