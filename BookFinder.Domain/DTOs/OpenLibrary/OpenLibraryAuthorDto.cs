using System.Text.Json.Serialization;
namespace BookFinder.Domain.DTOs.OpenLibrary;

public class OpenLibraryAuthorDto
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}