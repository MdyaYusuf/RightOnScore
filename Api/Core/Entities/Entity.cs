namespace Api.Core.Entities;

public abstract class Entity<TId> where TId : notnull
{
  protected Entity()
  {
    CreatedDate = DateTime.Now;
  }

  protected Entity(TId id) : this()
  {
    Id = id;
  }

  public TId Id { get; protected set; } = default!;
  public DateTime CreatedDate { get; set; }
  public DateTime? UpdatedDate { get; set; }
}