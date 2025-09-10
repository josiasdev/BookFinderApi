using BookFinder.Infrastructure.Services.Models;
using System.Net.Http.Json;
namespace BookFinder.Infrastructure.Services.OpenLibrary;

public class OpenLibraryService : IOpenLibraryService
{
    private readonly HttpClient _httpClient;

    public OpenLibraryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://openlibrary.org/");
    }

    public async Task<OpenLibrarySearchResponse?> SearchBooksByAuthorAsync(string authorName)
    {
        try
        {
            var formattedAuthorName = authorName.ToLower().Replace(" ", "+");
            var requestUri = $"search.json?author={formattedAuthorName}";

            var response = await _httpClient.GetFromJsonAsync<OpenLibrarySearchResponse>(requestUri);
            return response;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
            return null;
        }
    }
}