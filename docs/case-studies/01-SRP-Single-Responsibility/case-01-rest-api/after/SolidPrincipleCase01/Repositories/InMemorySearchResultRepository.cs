using SolidPrincipleCase01.Models;

public class InMemorySearchResultRepository : ISearchResultRepository
{
    private static readonly List<SearchResult> _searchResults = new();
    private static int _nextId = 1;

    public Task SaveSearchResultsAsync(List<SearchResult> results)
    {
        foreach (var result in results)
        {
            result.Id = _nextId++;
            _searchResults.Add(result);
        }

        return Task.CompletedTask;
    }

    public Task<List<SearchResult>> GetSearchResultsByKeywordAsync(string keyword)
    {
        var results = _searchResults
            .Where(r => r.Keyword.Equals(keyword, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(r => r.SearchedAt)
            .ToList();

        return Task.FromResult(results);
    }

    public Task<List<SearchResult>> GetAllSearchResultsAsync()
    {
        return Task.FromResult(_searchResults.OrderByDescending(r => r.SearchedAt).ToList());
    }
}
