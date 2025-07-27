using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinTrack.DataAccess;

public static class DataAccessModuleExtension
{
    public static void AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FinDbContext>(options =>
        {
            string? connectionString = configuration.GetConnectionString("FinTrack");
            if (connectionString == null)
            {
                throw new Exception("Invalid db connection string (null)");
            }

            options.UseNpgsql(connectionString).UseLazyLoadingProxies();
        });
    }
}