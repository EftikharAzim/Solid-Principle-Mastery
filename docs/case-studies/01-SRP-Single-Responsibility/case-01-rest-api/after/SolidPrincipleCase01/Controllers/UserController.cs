using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolidPrincipleCase01.Models;
using SolidPrincipleCase01.Services;

namespace SolidPrincipleCase01.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IPasswordValidationService _passwordValidator;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IPasswordValidationService passwordValidator,
            IUserRepository userRepository,
            ILogger<UsersController> logger
        )
        {
            _passwordValidator = passwordValidator;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login attempt for username: {Username}", request.Username);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isValid = await _passwordValidator.ValidatePasswordAsync(
                request.Username,
                request.Password
            );

            if (isValid)
            {
                var token = _passwordValidator.GenerateJwtToken(request.Username);
                _logger.LogInformation(
                    "Login successful for username: {Username}",
                    request.Username
                );

                return Ok(new LoginResponse { Token = token, Message = "Login successful" });
            }

            _logger.LogWarning("Login failed for username: {Username}", request.Username);
            return Unauthorized(new LoginResponse { Message = "Invalid credentials" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _logger.LogInformation("User logout");
            return Ok(new { Message = "Logout successful" });
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            // Remove password hashes from response
            var safeUsers = users
                .Select(u => new User
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt,
                })
                .ToList();

            return Ok(safeUsers);
        }
    }
}
