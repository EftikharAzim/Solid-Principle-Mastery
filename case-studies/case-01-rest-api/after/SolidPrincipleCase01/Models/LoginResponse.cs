using System.Text.Json.Serialization;

namespace SolidPrincipleCase01.Models;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
