using System;

namespace SolidPrincipleCase01.Models;

public class SearchResult
{
    public int Id { get; set; }
    public string Keyword { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
    public DateTime SearchedAt { get; set; }
}
