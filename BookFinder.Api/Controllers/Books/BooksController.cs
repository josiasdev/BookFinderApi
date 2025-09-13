using BookFinder.Domain.DTOs.Author;
using BookFinder.Domain.DTOs.Book;
using BookFinder.Domain.DTOs.Paginacao;
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
    /// Busca todos os autores e seus livros de forma paginada.
    /// </summary>
    /// <param name="pageNumber">O número da página a ser retornada. O padrão é 1.</param>
    /// <param name="pageSize">O número de itens por página. O padrão é 10.</param>
    /// <returns>Uma lista paginada de autores com seus livros e metadados de paginação.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponseDto<AuthorDto>), 200)]
    public async Task<IActionResult> GetAllAuthorsWithBooks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {

        var totalCount = await _context.Authors.CountAsync();
        
        var authorsFromDb = await _context.Authors
            .Include(a => a.Books)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
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

        var paginationMetadata = new PaginationMetadata
        {
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPage = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
        var response = new PaginatedResponseDto<AuthorDto>
        {
            Data = authorsDto,
            Pagination = paginationMetadata
        };
        
        return Ok(response);
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

    [HttpGet("count")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetTotalBookCount()
    {
        try
        {
            var totalBooks = await _context.Books.CountAsync();

            return Ok(new { totalBooks });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Busca na Open Library uma lista paginada de livros publicados em um ano específico.
    /// </summary>
    /// <param name="year">Ano de publicação para a busca.</param>
    /// <param name="pageNumber">O número da página a ser retornada.</param>
    /// <param name="pageSize">O número de itens por página.</param>
    /// <returns>Uma lista paginada de livros encontrados para o ano especificado.</returns>
    [HttpGet("/{year}")]
    [ProducesResponseType(typeof(PaginatedResponseDto<BookByYearDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetBooksByYear(
        int year,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var limit = pageSize;
        var offset = (pageNumber - 1) * pageSize;

        var openLibraryResponse = await _openLibraryService.GetBooksByYearAsync(year, limit, offset);

        if (openLibraryResponse == null || openLibraryResponse.Works.Count == 0)
        {
            return NotFound($"Nenhum livro encontrado para o ano {year}.");
        }

        var booksDto = openLibraryResponse.Works.Select(work => new BookByYearDto
        {
            Title = work.Title,
            PublishYear = work.FirstPublishYear,
            Authors = work.Authors.Select(a => a.Name).ToList(),
            OpenLibraryKey = work.Key
        }).ToList();

        var paginationMetadata = new PaginationMetadata
        {
            TotalCount = openLibraryResponse.WorkCount,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPage = (int)Math.Ceiling(openLibraryResponse.WorkCount / (double)pageSize)
        };

        var response = new PaginatedResponseDto<BookByYearDto>
        {
            Data = booksDto,
            Pagination = paginationMetadata
        };

        return Ok(response);
    }

}