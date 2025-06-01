using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Respositories;
using TaskManager.Infrastructure.Utility;

namespace TaskManager.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserRepository _userRepo;
    private readonly JwtService _jwtService;

    public AuthController(UserRepository userRepo, JwtService jwtService)
    {
        _userRepo = userRepo;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _userRepo.GetUserByUsername(request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { Message = "Invalid username or password" });
        }

        var token = _jwtService.GenerateToken(user);
        return Ok(new { token });
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

        _userRepo.AddUser(newUser);
        return Ok(new { Message = "User registered successfully" });
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

