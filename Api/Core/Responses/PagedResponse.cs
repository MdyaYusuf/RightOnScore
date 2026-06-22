namespace Api.Core.Responses;

public class PagedResponse<T>
{
  public List<T> Items { get; set; }
  public int CurrentPage { get; set; }
  public int PageSize { get; set; }
  public int TotalPages { get; set; }
  public int TotalCount { get; set; }
  public bool HasPreviousPage => CurrentPage > 1;
  public bool HasNextPage => CurrentPage < TotalPages;

  public PagedResponse(List<T> items, int totalCount, int pageNumber, int pageSize)
  {
    Items = items;
    TotalCount = totalCount;
    CurrentPage = pageNumber;
    PageSize = pageSize;
    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
  }
}