using Api.Core.Repositories;
using Api.Data;

namespace Api.Features.Roles;

public class EfRoleRepository : EfBaseRepository<BaseDbContext, Role, int>, IRoleRepository
{
  public EfRoleRepository(BaseDbContext context) : base(context)
  {

  }
}