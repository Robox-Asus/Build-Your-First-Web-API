using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(IUnitOfWork _work) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _work.UserRepository.GetUserByUsername(request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { Message = "Invalid username or password" });
        }

        var token = _work.JwtService.GenerateToken(user);
        return Ok(new Token { token = token });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        // Hash the password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = new User
        {
            Username = request.Username,
            PasswordHash = hashedPassword,
            Role = request.Role
        };

        _work.UserRepository.AddUser(newUser);
        return Ok(new RegisterResponse { Message = "User registered successfully" });
    }
}

public class LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
public class RegisterRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }
}

public class RegisterResponse
{
    public string Message { get; set; } = default!;
}

public class Token
{
    public string token { get; set; } = default!;
}
