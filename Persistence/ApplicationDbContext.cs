using Microsoft.EntityFrameworkCore;
using Persistence.EntityConfigs;

namespace Persistence;

public class ApplicationDbContext : DbContext
{
    #region Constructor
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    #endregion Constructor

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new AdministratorConfig());
    }
}