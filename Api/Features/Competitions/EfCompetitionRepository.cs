using Api.Core.Repositories;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Competitions;

public class EfCompetitionRepository : EfBaseRepository<BaseDbContext, Competition, Guid>, ICompetitionRepository
{
  public EfCompetitionRepository(BaseDbContext context) : base(context)
  {

  }

  public async Task<List<Competition>> GetActiveCompetitionsAsync(CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .Where(c => c.IsActive)
      .OrderBy(c => c.Name)
      .ToListAsync(cancellationToken);
  }

  public async Task<Competition?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
  }
}
