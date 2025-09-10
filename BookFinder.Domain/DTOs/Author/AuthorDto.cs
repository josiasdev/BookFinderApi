using BookFinder.Domain.DTOs.Book;

namespace BookFinder.Domain.DTOs.Author;

public class AuthorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OpenLibraryKey { get; set; } = string.Empty;
    public List<BookDto> Books { get; set; } = new();
}