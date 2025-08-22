using System;
using System.Text.Json.Serialization;

namespace SolidPrincipleCase01.Models;

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
