using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data.Contexts;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseSqlServer("Server=tcp:jacsjo-sqlserver.database.windows.net,1433;Initial Catalog=jacsjo-sqldb;Persist Security Info=False;User ID=SqlAdmin;Password=GissaMig123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        return new DataContext(optionsBuilder.Options);
    }
}
