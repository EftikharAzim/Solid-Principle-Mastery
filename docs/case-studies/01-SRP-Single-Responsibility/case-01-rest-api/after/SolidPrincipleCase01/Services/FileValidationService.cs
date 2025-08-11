using SolidPrincipleCase01.Services;

public class FileValidationService : IFileValidationService
{
    public bool IsValidCsvFormat(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        // Check file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".csv")
            return false;

        // Check content type
        return file.ContentType == "text/csv" || file.ContentType == "application/vnd.ms-excel";
    }

    public async Task<List<string>> ExtractKeywordsAsync(Stream fileStream)
    {
        var keywords = new List<string>();
        using var reader = new StreamReader(fileStream);

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                // Split by comma and clean up
                var lineKeywords = line.Split(',')
                    .Select(k => k.Trim())
                    .Where(k => !string.IsNullOrEmpty(k));

                keywords.AddRange(lineKeywords);
            }
        }

        return keywords.Distinct().ToList();
    }
}
