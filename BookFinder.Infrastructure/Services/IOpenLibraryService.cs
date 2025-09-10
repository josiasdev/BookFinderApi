using BookFinder.Infrastructure.Services.Models;
namespace BookFinder.Infrastructure.Services;

public interface IOpenLibraryService
{
    Task<OpenLibrarySearchResponse> SearchBooksByAuthorAsync(string searchTerm);
}