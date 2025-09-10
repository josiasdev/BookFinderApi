namespace BookFinder.Domain.Models;

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? FirstPublishYear { get; set; }
    public Guid AuthorId { get; set; }
    public Author? Author { get; set; }
}