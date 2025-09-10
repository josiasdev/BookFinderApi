using System.Text.Json.Serialization;
namespace BookFinder.Infrastructure.Services.Models;

public class OpenLibrarySearchResponse
{
    [JsonPropertyName("docs")]
    public List<BookDocument> Docs { get; set; } = new();
}