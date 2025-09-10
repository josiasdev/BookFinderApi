namespace BookFinder.Domain.DTOs.Book;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? FirstPublishYear { get; set; }
}