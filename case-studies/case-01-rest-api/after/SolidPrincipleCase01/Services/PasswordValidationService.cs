using SolidPrincipleCase01.Services;

public class PasswordValidationService : IPasswordValidationService
{
    private readonly IUserRepository _userRepository;

    public PasswordValidationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> ValidatePasswordAsync(string username, string password)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);
        if (user == null)
            return false;

        // In real app, you'd use BCrypt or similar for password hashing
        // For demo, we'll do simple comparison
        return user.PasswordHash == password + "_hashed";
    }

    public string GenerateJwtToken(string username)
    {
        // Simplified JWT token generation for demo
        return $"jwt_token_for_{username}_{DateTime.UtcNow.Ticks}";
    }
}
