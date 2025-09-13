using BookFinder.Infrastructure.Services.Models;
using BookFinder.Domain.DTOs.OpenLibrary;
namespace BookFinder.Infrastructure.Services.OpenLibrary;

public interface IOpenLibraryService
{
    Task<OpenLibrarySearchResponse?> SearchBooksByAuthorAsync(string authorName);
    Task<OpenLibrarySubjectResponseDto?> GetBooksByYearAsync(int year, int limit, int offset);
}