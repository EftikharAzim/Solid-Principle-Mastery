# Authentication System

A demonstration of SOLID principles through a flexible authentication system that supports multiple authentication providers without modifying core logic.

## 🎯 Problem Solved

**Challenge**: Create an authentication system that can integrate different providers (database, OAuth, SAML) without changing the main authentication logic when switching or adding new providers.

**Solution**: Applied all five SOLID principles to create a flexible, extensible architecture.

## 🏗️ Architecture

```
AuthenticationService (Coordinator)
    ↓
IAuthenticationProvider (Abstraction)
    ↓
┌─────────────────┬──────────────────┐
│DatabaseProvider │   OAuthProvider  │
└─────────────────┴──────────────────┘
```

## 🔧 SOLID Principles Applied

| Principle | How Applied                                 | Benefit                                          |
| --------- | ------------------------------------------- | ------------------------------------------------ |
| **SRP**   | Each class has single responsibility        | Easy to modify and test                          |
| **OCP**   | Open for extension, closed for modification | Add new providers without changing existing code |
| **LSP**   | All providers are substitutable             | Any provider can replace another                 |
| **ISP**   | Focused, minimal interfaces                 | No unnecessary dependencies                      |
| **DIP**   | Depend on abstractions, not concretions     | Easy testing and swapping implementations        |

## 🚀 Key Features

- **Flexible Parameters**: Each provider accepts different authentication data
- **Consistent Results**: All providers return unified `AuthenticationResult`
- **Easy Extension**: Add new providers by implementing `IAuthenticationProvider`
- **Dependency Injection**: Testable and configurable
- **Error Handling**: Comprehensive validation and error reporting

## 💻 Quick Example

```csharp
// Database Authentication
IUserRepository repo = new InMemoryUserRepository();
IAuthenticationProvider provider = new DatabaseAuthenticationProvider(repo);
var authService = new AuthenticationService(provider);

var request = new AuthenticationRequest();
request.Parameters["username"] = "testuser";
request.Parameters["password"] = "password123";

var result = authService.Login(request);

// OAuth Authentication - same service, different provider!
IAuthenticationProvider oauthProvider = new OAuthAuthenticationProvider();
var oauthService = new AuthenticationService(oauthProvider);

var oauthRequest = new AuthenticationRequest();
oauthRequest.Parameters["authCode"] = "valid_auth_code_123";
oauthRequest.Parameters["redirectUri"] = "https://myapp.com/callback";

var oauthResult = oauthService.Login(oauthRequest);
```

## 🎓 Learning Outcomes

This implementation demonstrates:

1. **Real-world SOLID application** - Not just theory, but practical problem-solving
2. **Flexible design patterns** - Handle different authentication methods uniformly
3. **Professional error handling** - Comprehensive validation and user-friendly messages
4. **Testable architecture** - Easy to unit test each component in isolation
5. **Extensible system** - Add SAML, JWT, API keys without breaking existing code

## 📈 Future Extensions

Adding new authentication providers is simple:

```csharp
public class SAMLAuthenticationProvider : IAuthenticationProvider
{
    public string ProviderName => "SAML";

    public AuthenticationResult Authenticate(AuthenticationRequest request)
    {
        // SAML-specific implementation
        // No changes needed to existing code!
    }
}
```
