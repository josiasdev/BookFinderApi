using System.Text.Json.Serialization;

namespace BookFinder.Infrastructure.Services.Models;

public class BookDocument
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("author_name")]
    public List<string> AuthorName { get; set; } = new();

    [JsonPropertyName("author_key")]
    public List<string> AuthorKey { get; set; } = new();

    [JsonPropertyName("first_publish_year")]
    public int? FirstPublishYear { get; set; }
}