using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        string connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
        builder.UseSqlServer(connectionString);
        return new ApplicationDbContext(builder.Options);
    }
}