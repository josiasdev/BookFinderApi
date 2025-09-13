namespace BookFinder.Domain.DTOs.Paginacao;

public class PaginatedResponseDto<T>
{
    public List<T> Data { get; set; } = new();
    public PaginationMetadata Pagination { get; set; } = new();

}