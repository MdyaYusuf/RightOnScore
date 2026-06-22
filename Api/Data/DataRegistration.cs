using Api.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public static class DataRegistration
{
  public static IServiceCollection AddDataDependencies(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("SqlConnection");

    services.AddDbContext<BaseDbContext>(options =>
    {
      options.UseSqlServer(connectionString);
    });

    services.AddScoped<IUnitOfWork, UnitOfWork>();

    return services;
  }
}