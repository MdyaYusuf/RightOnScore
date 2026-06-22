namespace Api.Core.Responses;

public class CursorPagedResponse<T>
{
  public List<T> Items { get; set; } = new();
  public DateTime? NextCursorDate { get; set; }
  public decimal? NextCursorValue { get; set; }
  public Guid? NextCursorId { get; set; }
  public bool HasNextPage { get; set; }
}