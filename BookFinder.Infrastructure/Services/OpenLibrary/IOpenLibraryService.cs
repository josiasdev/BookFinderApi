using BookFinder.Infrastructure.Services.Models;
namespace BookFinder.Infrastructure.Services.OpenLibrary;

public interface IOpenLibraryService
{
    Task<OpenLibrarySearchResponse?> SearchBooksByAuthorAsync(string authorName);
}