using SolidPrincipleCase01.Models;

public interface IUserRepository
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User> CreateUserAsync(User user);
    Task<List<User>> GetAllUsersAsync();
}
