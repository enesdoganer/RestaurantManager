using Microsoft.EntityFrameworkCore;

namespace RestaurantManager.Core.Data;

public static class DbContextFactory
{
    public static DbContext Create(string connectionString)
    {
        var options = new DbContextOptionsBuilder<DbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new DbContext(options);
    }
}