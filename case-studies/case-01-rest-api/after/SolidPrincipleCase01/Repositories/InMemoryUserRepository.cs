using SolidPrincipleCase01.Models;

public class InMemoryUserRepository : IUserRepository
{
    private static readonly List<User> _users = new()
    {
        new User
        {
            Id = 1,
            Username = "demo",
            Email = "demo@example.com",
            PasswordHash = "password_hashed",
            CreatedAt = DateTime.UtcNow,
        },
        new User
        {
            Id = 2,
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = "admin123_hashed",
            CreatedAt = DateTime.UtcNow,
        },
        new User
        {
            Id = 3,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "test123_hashed",
            CreatedAt = DateTime.UtcNow,
        },
    };

    public Task<User?> GetUserByUsernameAsync(string username)
    {
        var user = _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
        );
        return Task.FromResult(user);
    }

    public Task<User> CreateUserAsync(User user)
    {
        user.Id = _users.Count + 1;
        user.CreatedAt = DateTime.UtcNow;
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task<List<User>> GetAllUsersAsync()
    {
        return Task.FromResult(_users.ToList());
    }
}
