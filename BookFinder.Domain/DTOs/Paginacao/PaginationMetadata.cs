namespace BookFinder.Domain.DTOs.Paginacao;

public class PaginationMetadata
{
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPage { get; set; }
    public int PageSize { get; set; }
}