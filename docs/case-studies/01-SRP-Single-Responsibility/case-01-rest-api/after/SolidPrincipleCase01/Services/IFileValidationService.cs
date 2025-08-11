namespace SolidPrincipleCase01.Services
{
    public interface IFileValidationService
    {
        bool IsValidCsvFormat(IFormFile file);
        Task<List<string>> ExtractKeywordsAsync(Stream fileStream);
    }
}
