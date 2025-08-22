using SolidPrincipleCase01.Models;

public interface ISearchResultRepository
{
    Task SaveSearchResultsAsync(List<SearchResult> results);
    Task<List<SearchResult>> GetSearchResultsByKeywordAsync(string keyword);
    Task<List<SearchResult>> GetAllSearchResultsAsync();
}
