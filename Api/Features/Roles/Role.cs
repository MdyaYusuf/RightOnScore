using System.Diagnostics.CodeAnalysis;
using Api.Core.Entities;
using Api.Features.Users;

namespace Api.Features.Roles;

public class Role : Entity<int>
{
  [SetsRequiredMembers]
  public Role()
  {
    Users = new HashSet<User>();

    Name = default!;
  }

  public required string Name { get; set; }

  // Relationship properties
  public virtual ICollection<User> Users { get; set; }
}