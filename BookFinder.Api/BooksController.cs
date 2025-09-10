using BookFinder.Domain;
using BookFinder.Infrastructure.Data;
using BookFinder.Infrastructure.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookFinder.Api.Controller;

[ApiController]
[Route ("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IOpenLibraryService _openLibraryService;
    private readonly ApplicationDbContext _dbContext;

    public BooksController(IOpenLibraryService openLibraryService, ApplicationDbContext dbContext)
    {
        _openLibraryService = openLibraryService;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Searches for books by an author, fetches data from Open Library,
    /// and saves the new information to the local database.
    /// </summary>
    /// <param name="authorName">The name of the author to search for.</param>
    /// <returns>The author and their list of books found.</returns>
    [HttpPost("search-and-save/{authorName}")]
    [ProducesResponseType(typeof(Author), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SearchAndSaveAuthorBooks(string authorName)
    {
        var searchResult = await _openLibraryService.SearchBooksByAuthorAsync(authorName);

        if (searchResult == null || searchResult.Docs.Count == 0)
        {
            return NotFound($"No books found for author '{authorName}'.");
        }
        var firstBook = searchResult.Docs.First();
        var authorKey = firstBook.AuthorKey.First();

        var author = await _dbContext.Authors
            .Include(a => a.books)
            .FirstOrDefaultAsync(a => a.OpenLibraryKey == authorKey);

        if (author == null)
        {
            author = new Author
            {
                Id = Guid.NewGuid(),
                Name = firstBook.AuthorName.First(),
                OpenLibraryKey = authorKey
            };
            _dbContext.Authors.Add(author);
        }

        foreach (var bookDoc in searchResult.Docs)
        {
            var bookExists = author.books.Any(b => b.Title == bookDoc.Title);
            if (!bookExists)
            {
                author.books.Add(new Book
                {
                    Id = Guid.NewGuid(),
                    Title = bookDoc.Title,
                    FirstPublishYear = bookDoc.FirstPublishYear,
                });
            }
        }

        await _dbContext.SaveChangesAsync();
        return Ok(author);
    }
}