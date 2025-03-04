using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        string connectionString = "Server=DESKTOP-I2AV7J8\\SQLEXPRESS;Database=SimpleProject;Trusted_Connection=True;TrustServerCertificate=True";
        builder.UseSqlServer(connectionString);
        return new ApplicationDbContext(builder.Options);
    }
}