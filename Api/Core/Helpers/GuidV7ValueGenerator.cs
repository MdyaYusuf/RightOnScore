using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Api.Core.Helpers;

public class GuidV7ValueGenerator : ValueGenerator<Guid>
{
  public override bool GeneratesTemporaryValues => false;

  public override Guid Next(EntityEntry entry)
  {
    return Guid.CreateVersion7();
  }
}