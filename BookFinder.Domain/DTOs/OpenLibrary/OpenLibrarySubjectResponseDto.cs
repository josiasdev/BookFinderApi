using System.Text.Json.Serialization;
namespace BookFinder.Domain.DTOs.OpenLibrary;

public class OpenLibrarySubjectResponseDto
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("work_count")]
    public int WorkCount { get; set; }

    [JsonPropertyName("works")]
    public List<OpenLibraryWorkDto> Works { get; set; } = new();
}