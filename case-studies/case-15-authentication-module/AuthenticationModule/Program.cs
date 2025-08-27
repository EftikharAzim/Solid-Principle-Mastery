using System;
using System.Collections.Generic;
using System.Linq;

// Core Domain Models
public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public Dictionary<string, string> Claims { get; set; } = new();
}

public class AuthenticationResult
{
    public bool IsAuthenticated { get; set; }
    public string UserName { get; set; }
    public string ErrorMessage { get; set; }
    public Dictionary<string, string> Claims { get; set; } = new();
}

public class AuthenticationRequest
{
    public Dictionary<string, object> Parameters { get; set; } = new();
}

// Supporting Models for OAuth
public class TokenResponse
{
    public bool IsValid { get; set; }
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
}

public class UserInfo
{
    public string Email { get; set; }
    public string Name { get; set; }
}

// Repository Interface and Implementation (SRP + DIP)
public interface IUserRepository
{
    User? GetUser(string username);
}

public class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<string, User> _users = new();

    public InMemoryUserRepository()
    {
        // Seed data for testing
        _users["admin"] = new User
        {
            Username = "admin",
            Password = "password123",
            Claims = new() { { "role", "administrator" }, { "department", "IT" } }
        };
        _users["testuser"] = new User
        {
            Username = "testuser",
            Password = "password123",
            Claims = new() { { "role", "user" }, { "department", "Sales" } }
        };
        _users["john.doe"] = new User
        {
            Username = "john.doe",
            Password = "securepass456",
            Claims = new() { { "role", "manager" }, { "department", "Marketing" } }
        };
    }

    public User? GetUser(string username) => _users.TryGetValue(username, out var user) ? user : null;
}

// Core Authentication Interface (OCP + LSP)
public interface IAuthenticationProvider
{
    AuthenticationResult Authenticate(AuthenticationRequest request);
    string ProviderName { get; }
}

// Database Authentication Provider (SRP + DIP)
public class DatabaseAuthenticationProvider : IAuthenticationProvider
{
    private readonly IUserRepository _userRepository;

    public DatabaseAuthenticationProvider(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public string ProviderName => "Database";

    public AuthenticationResult Authenticate(AuthenticationRequest request)
    {
        try
        {
            // Validate input parameters
            if (!request.Parameters.TryGetValue("username", out var usernameObj) ||
                !request.Parameters.TryGetValue("password", out var passwordObj))
            {
                return new AuthenticationResult
                {
                    IsAuthenticated = false,
                    ErrorMessage = "Username and password are required for database authentication"
                };
            }

            var username = usernameObj?.ToString();
            var password = passwordObj?.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return new AuthenticationResult
                {
                    IsAuthenticated = false,
                    ErrorMessage = "Username and password cannot be empty"
                };
            }

            // Authenticate user
            var user = _userRepository.GetUser(username);
            if (user == null || user.Password != password)
            {
                return new AuthenticationResult
                {
                    IsAuthenticated = false,
                    ErrorMessage = "Invalid username or password"
                };
            }

            // Success - return authenticated result with claims
            return new AuthenticationResult
            {
                IsAuthenticated = true,
                UserName = user.Username,
                Claims = new Dictionary<string, string>(user.Claims)
                {
                    ["provider"] = ProviderName,
                    ["authTime"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
                },
                ErrorMessage = null
            };
        }
        catch (Exception ex)
        {
            return new AuthenticationResult
            {
                IsAuthenticated = false,
                ErrorMessage = $"Database authentication error: {ex.Message}"
            };
        }
    }
}

// OAuth Authentication Provider (SRP + OCP)
public class OAuthAuthenticationProvider : IAuthenticationProvider
{
    public string ProviderName => "OAuth";

    public AuthenticationResult Authenticate(AuthenticationRequest request)
    {
        try
        {
            // OAuth requires different parameters than database authentication
            if (!request.Parameters.TryGetValue("authCode", out var authCodeObj) ||
                !request.Parameters.TryGetValue("redirectUri", out var redirectUriObj))
            {
                return new AuthenticationResult
                {
                    IsAuthenticated = false,
                    ErrorMessage = "Authorization code and redirect URI are required for OAuth authentication"
                };
            }

            var authCode = authCodeObj?.ToString();
            var redirectUri = redirectUriObj?.ToString();

            if (string.IsNullOrEmpty(authCode) || string.IsNullOrEmpty(redirectUri))
            {
                return new AuthenticationResult
                {
                    IsAuthenticated = false,
                    ErrorMessage = "Authorization code and redirect URI cannot be empty"
                };
            }

            // Step 1: Exchange authorization code for access token
            var tokenResponse = ExchangeCodeForToken(authCode, redirectUri);

            if (tokenResponse == null || !tokenResponse.IsValid)
            {
                return new AuthenticationResult
                {
                    IsAuthenticated = false,
                    ErrorMessage = "Invalid authorization code or token exchange failed"
                };
            }

            // Step 2: Get user information using access token
            var userInfo = GetUserInfoFromToken(tokenResponse.AccessToken);

            if (userInfo == null)
            {
                return new AuthenticationResult
                {
                    IsAuthenticated = false,
                    ErrorMessage = "Failed to retrieve user information from OAuth provider"
                };
            }

            // Success - return authenticated result with OAuth-specific claims
            return new AuthenticationResult
            {
                IsAuthenticated = true,
                UserName = userInfo.Email,
                Claims = new Dictionary<string, string>
                {
                    ["email"] = userInfo.Email,
                    ["name"] = userInfo.Name,
                    ["provider"] = ProviderName,
                    ["tokenType"] = tokenResponse.TokenType,
                    ["authTime"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
                },
                ErrorMessage = null
            };
        }
        catch (Exception ex)
        {
            return new AuthenticationResult
            {
                IsAuthenticated = false,
                ErrorMessage = $"OAuth authentication error: {ex.Message}"
            };
        }
    }

    // Simulate OAuth token exchange (in real implementation, this would call OAuth provider's token endpoint)
    private TokenResponse? ExchangeCodeForToken(string authCode, string redirectUri)
    {
        // Simulate successful token exchange for valid codes
        var validCodes = new[] { "valid_auth_code_123", "google_auth_code_456", "github_auth_code_789" };

        if (validCodes.Contains(authCode) && !string.IsNullOrEmpty(redirectUri))
        {
            return new TokenResponse
            {
                IsValid = true,
                AccessToken = $"oauth_access_token_{Guid.NewGuid():N}",
                TokenType = "Bearer"
            };
        }

        return new TokenResponse { IsValid = false };
    }

    // Simulate getting user info from OAuth provider (in real implementation, this would call user info endpoint)
    private UserInfo? GetUserInfoFromToken(string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
            return null;

        // Simulate different users based on token patterns
        if (accessToken.Contains("oauth_access_token"))
        {
            return new UserInfo
            {
                Email = "oauth.user@example.com",
                Name = "OAuth Test User"
            };
        }

        return null;
    }
}

// Main Authentication Service (SRP + DIP)
public class AuthenticationService
{
    private readonly IAuthenticationProvider _authenticationProvider;

    public AuthenticationService(IAuthenticationProvider authenticationProvider)
    {
        _authenticationProvider = authenticationProvider ?? throw new ArgumentNullException(nameof(authenticationProvider));
    }

    public AuthenticationResult Login(AuthenticationRequest request)
    {
        if (request == null)
        {
            return new AuthenticationResult
            {
                IsAuthenticated = false,
                ErrorMessage = "Authentication request cannot be null"
            };
        }

        return _authenticationProvider.Authenticate(request);
    }

    public string GetProviderName() => _authenticationProvider.ProviderName;
}

// Demo Program
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("🔐 SOLID Authentication System Demo");
        Console.WriteLine("=====================================\n");

        // Test Database Authentication
        TestDatabaseAuthentication();

        Console.WriteLine("\n" + new string('-', 50) + "\n");

        // Test OAuth Authentication
        TestOAuthAuthentication();

        Console.WriteLine("\n🎯 Demo completed!");
    }

    private static void TestDatabaseAuthentication()
    {
        Console.WriteLine("📊 DATABASE AUTHENTICATION TESTS");
        Console.WriteLine("=================================");

        // Setup dependencies using Dependency Injection
        IUserRepository userRepo = new InMemoryUserRepository();
        IAuthenticationProvider authProvider = new DatabaseAuthenticationProvider(userRepo);
        var authService = new AuthenticationService(authProvider);

        Console.WriteLine($"Provider: {authService.GetProviderName()}\n");

        // Test Case 1: Successful login
        Console.WriteLine("Test 1: Valid credentials");
        var request1 = new AuthenticationRequest();
        request1.Parameters["username"] = "testuser";
        request1.Parameters["password"] = "password123";

        var result1 = authService.Login(request1);
        PrintAuthenticationResult(result1);

        // Test Case 2: Invalid password
        Console.WriteLine("\nTest 2: Invalid password");
        var request2 = new AuthenticationRequest();
        request2.Parameters["username"] = "testuser";
        request2.Parameters["password"] = "wrongpassword";

        var result2 = authService.Login(request2);
        PrintAuthenticationResult(result2);

        // Test Case 3: Non-existent user
        Console.WriteLine("\nTest 3: Non-existent user");
        var request3 = new AuthenticationRequest();
        request3.Parameters["username"] = "nonexistent";
        request3.Parameters["password"] = "password123";

        var result3 = authService.Login(request3);
        PrintAuthenticationResult(result3);

        // Test Case 4: Missing parameters
        Console.WriteLine("\nTest 4: Missing parameters");
        var request4 = new AuthenticationRequest();
        request4.Parameters["username"] = "testuser";
        // Missing password parameter

        var result4 = authService.Login(request4);
        PrintAuthenticationResult(result4);
    }

    private static void TestOAuthAuthentication()
    {
        Console.WriteLine("🔗 OAUTH AUTHENTICATION TESTS");
        Console.WriteLine("==============================");

        // Setup OAuth provider (notice how easy it is to switch providers!)
        IAuthenticationProvider authProvider = new OAuthAuthenticationProvider();
        var authService = new AuthenticationService(authProvider);

        Console.WriteLine($"Provider: {authService.GetProviderName()}\n");

        // Test Case 1: Successful OAuth flow
        Console.WriteLine("Test 1: Valid OAuth flow");
        var request1 = new AuthenticationRequest();
        request1.Parameters["authCode"] = "valid_auth_code_123";
        request1.Parameters["redirectUri"] = "https://myapp.com/oauth/callback";

        var result1 = authService.Login(request1);
        PrintAuthenticationResult(result1);

        // Test Case 2: Invalid authorization code
        Console.WriteLine("\nTest 2: Invalid authorization code");
        var request2 = new AuthenticationRequest();
        request2.Parameters["authCode"] = "invalid_auth_code";
        request2.Parameters["redirectUri"] = "https://myapp.com/oauth/callback";

        var result2 = authService.Login(request2);
        PrintAuthenticationResult(result2);

        // Test Case 3: Missing parameters
        Console.WriteLine("\nTest 3: Missing OAuth parameters");
        var request3 = new AuthenticationRequest();
        request3.Parameters["authCode"] = "valid_auth_code_123";
        // Missing redirectUri parameter

        var result3 = authService.Login(request3);
        PrintAuthenticationResult(result3);
    }

    private static void PrintAuthenticationResult(AuthenticationResult result)
    {
        if (result.IsAuthenticated)
        {
            Console.WriteLine($"✅ SUCCESS: User '{result.UserName}' authenticated successfully!");
            if (result.Claims.Any())
            {
                Console.WriteLine($"📋 Claims: {string.Join(", ", result.Claims.Select(c => $"{c.Key}={c.Value}"))}");
            }
        }
        else
        {
            Console.WriteLine($"❌ FAILED: {result.ErrorMessage}");
        }
    }
}