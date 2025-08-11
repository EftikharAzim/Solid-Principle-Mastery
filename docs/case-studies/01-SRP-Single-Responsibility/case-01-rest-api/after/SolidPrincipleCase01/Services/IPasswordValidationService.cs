namespace SolidPrincipleCase01.Services
{
    public interface IPasswordValidationService
    {
        Task<bool> ValidatePasswordAsync(string username, string password);
        string GenerateJwtToken(string username);
    }
}
