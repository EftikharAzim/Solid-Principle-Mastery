using SolidPrincipleCase01.Models;

public class GoogleSearchService : ISearchService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GoogleSearchService> _logger;

    public GoogleSearchService(HttpClient httpClient, ILogger<GoogleSearchService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<SearchResult>> SearchAsync(string keyword)
    {
        try
        {
            _logger.LogInformation("Searching for keyword: {Keyword}", keyword);

            // For demo purposes, we'll simulate Google search results
            // In real implementation, you'd call actual Google Custom Search API
            await Task.Delay(500); // Simulate network delay

            var results = GenerateMockSearchResults(keyword);

            _logger.LogInformation(
                "Found {Count} results for keyword: {Keyword}",
                results.Count,
                keyword
            );
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Search failed for keyword: {Keyword}", keyword);
            throw new SearchServiceException($"Search failed for keyword: {keyword}", ex);
        }
    }

    public async Task<List<SearchResult>> SearchMultipleAsync(List<string> keywords)
    {
        var allResults = new List<SearchResult>();

        foreach (var keyword in keywords)
        {
            var results = await SearchAsync(keyword);
            allResults.AddRange(results);

            // Rate limiting - don't overwhelm the API
            await Task.Delay(200);
        }

        return allResults;
    }

    private List<SearchResult> GenerateMockSearchResults(string keyword)
    {
        // Generate 3-5 mock search results per keyword
        var random = new Random();
        var resultCount = random.Next(3, 6);
        var results = new List<SearchResult>();

        for (int i = 1; i <= resultCount; i++)
        {
            results.Add(
                new SearchResult
                {
                    Id = 0, // Will be set by repository
                    Keyword = keyword,
                    Title = $"Result {i} for '{keyword}' - Sample Title",
                    Url = $"https://example.com/{keyword.ToLower()}/result-{i}",
                    Description =
                        $"This is a sample description for search result {i} related to '{keyword}'. "
                        + $"It contains relevant information about the topic.",
                    SearchedAt = DateTime.UtcNow,
                }
            );
        }

        return results;
    }
}
