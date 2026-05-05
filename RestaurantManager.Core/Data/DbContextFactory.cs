using Microsoft.EntityFrameworkCore;

namespace RestaurantManager.Core.Data;

public static class DbContextFactory
{
    public static RestaurantDbContext Create(string connectionString)
    {
        var options = new DbContextOptionsBuilder<RestaurantDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new RestaurantDbContext(options);
    }
}