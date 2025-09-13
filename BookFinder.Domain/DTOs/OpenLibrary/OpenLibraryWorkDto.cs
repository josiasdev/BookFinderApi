using System.Text.Json.Serialization;
namespace BookFinder.Domain.DTOs.OpenLibrary;

public class OpenLibraryWorkDto
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("authors")]
    public List<OpenLibraryAuthorDto> Authors { get; set; } = new();

    [JsonPropertyName("first_publish_year")]
    public int? FirstPublishYear { get; set; }
}