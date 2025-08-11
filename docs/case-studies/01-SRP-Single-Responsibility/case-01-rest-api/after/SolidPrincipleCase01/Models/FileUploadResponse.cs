using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SolidPrincipleCase01.Models;

public class FileUploadResponse
{
    public string Message { get; set; } = string.Empty;
    public int KeywordCount { get; set; }
    public int ResultCount { get; set; }
    public List<string> ProcessedKeywords { get; set; } = new();
}
