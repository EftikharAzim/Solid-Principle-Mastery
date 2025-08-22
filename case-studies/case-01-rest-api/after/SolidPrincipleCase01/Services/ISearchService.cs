using SolidPrincipleCase01.Models;

public interface ISearchService
{
    Task<List<SearchResult>> SearchAsync(string keyword);
    Task<List<SearchResult>> SearchMultipleAsync(List<string> keywords);
}
