using System.Reflection;
using Api.Core.Helpers;
using Api.Features.Roles;
using Api.Features.Users;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class BaseDbContext : DbContext
{
  public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
  {

  }

  public DbSet<Role> Roles { get; set; }
  public DbSet<User> Users { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
      var primaryKey = entityType.FindPrimaryKey();

      if (primaryKey != null && primaryKey.Properties.Count == 1)
      {
        var pkProperty = primaryKey.Properties[0];

        if (pkProperty.ClrType == typeof(Guid))
        {
          pkProperty.SetValueGeneratorFactory((_, _) => new GuidV7ValueGenerator());
        }
      }
    }
  }
}