using BookFinder.Domain.DTOs.Author;
using BookFinder.Domain.DTOs.Book;
using BookFinder.Infrastructure.Data;
using BookFinder.Infrastructure.Services.OpenLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BookFinder.Api.Controllers.Books;


[Authorize]
[ApiController]
[Route("/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IOpenLibraryService _openLibraryService;
    private readonly ApplicationDbContext _context;

    public BooksController(IOpenLibraryService openLibraryService, ApplicationDbContext context)
    {
        _openLibraryService = openLibraryService;
        _context = context;
    }

    /// <summary>
    /// Busca todos os autores e seus livros que já estão salvos no banco de dados.
    /// </summary>
    /// <returns>Uma lista de autores com seus respectivos livros.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<AuthorDto>), 200)]
    public async Task<IActionResult> GetAllAuthorsWithBooks()
    {

        var authorsFromDb = await _context.Authors
            .Include(a => a.Books)
            .ToListAsync();

        var authorsDto = authorsFromDb.Select(author => new AuthorDto
        {
            Id = author.Id,
            Name = author.Name,
            OpenLibraryKey = author.OpenLibraryKey,
            Books = author.Books.Select(book => new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                FirstPublishYear = book.FirstPublishYear
            }).ToList()
        }).ToList();

        return Ok(authorsDto);
    }

    /// <summary>
    /// Busca livros de um autor na API externa, salva no banco e retorna os dados salvos.
    /// </summary>
    /// <param name="authorName">O nome do autor a ser pesquisado.</param>
    /// <returns>O autor e sua lista de livros salvos.</returns>
    [HttpPost("search-and-save/{authorName}")]
    [ProducesResponseType(typeof(AuthorDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SearchAndSaveAuthorBooks(string authorName)
    {
        var searchResult = await _openLibraryService.SearchBooksByAuthorAsync(authorName);

        if (searchResult == null || searchResult.Docs.Count == 0)
        {
            return NotFound($"Nenhum livro encontrado para o autor '{authorName}'.");
        }

        var firstBook = searchResult.Docs.First();
        var authorKey = firstBook.AuthorKey.First();

        var author = await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.OpenLibraryKey == authorKey);

        if (author == null)
        {
            author = new()
            {
                Id = Guid.NewGuid(),
                Name = firstBook.AuthorName.First(),
                OpenLibraryKey = authorKey
            };
            _context.Authors.Add(author);
        }

        foreach (var bookDoc in searchResult.Docs)
        {
            var bookExists = author.Books.Any(b => b.Title == bookDoc.Title);
            if (!bookExists)
            {
                author.Books.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Title = bookDoc.Title,
                    FirstPublishYear = bookDoc.FirstPublishYear
                });
            }
        }

        await _context.SaveChangesAsync();

        var responseDto = new AuthorDto
        {
            Id = author.Id,
            Name = author.Name,
            OpenLibraryKey = author.OpenLibraryKey,
            Books = author.Books.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                FirstPublishYear = b.FirstPublishYear
            }).ToList()
        };

        return Ok(responseDto);
    }
    
    /// <summary>
    /// Busca um autor especifico pelo seu ID.
    /// </summary>
    [HttpGet("author/{id}")]
    [ProducesResponseType(typeof(AuthorDto), 200)]
    [ProducesResponseType(404)]
    
    public async Task<IActionResult> GetAuthorById(Guid id)
    {
        var author = await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (author == null)
        {
            return NotFound();
        }

        var responseDto = new AuthorDto
        {
            Id = author.Id, Name = author.Name, OpenLibraryKey = author.OpenLibraryKey,
            Books = author.Books.Select(b => new BookDto { Id = b.Id, Title = b.Title, FirstPublishYear = b.FirstPublishYear }).ToList()
        };

        return Ok(responseDto);
    }
    
    /// <summary>
    /// Atualiza o nome de um autor existente.
    /// </summary>
    [HttpPut("author/{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateAuthor(Guid id, [FromBody] UpdateAuthorDto authorDto)
    {
        var author = await _context.Authors.FindAsync(id);

        if (author == null)
        {
            return NotFound();
        }

        author.Name = authorDto.Name; 
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    /// <summary>
    /// Deleta um autor e todos os seus livros associados.
    /// </summary>
    [HttpDelete("author/{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteAuthor(Guid id)
    {
        var author = await _context.Authors.FindAsync(id);

        if (author == null)
        {
            return NotFound();
        }

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync(); 

        return NoContent(); 
    }
    
}