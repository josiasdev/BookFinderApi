namespace BookFinder.Domain.DTOs.Book;

public class BookByYearDto
{
    public string Title { get; set; } = string.Empty;
    public int? PublishYear { get; set; }
    public List<string> Authors { get; set; } = new();
    public string OpenLibraryKey { get; set; } = string.Empty;
}