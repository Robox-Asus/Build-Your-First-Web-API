namespace TaskManager.Tests.Controllers;

using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.API.Controllers;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Respositories;
using TaskManager.Infrastructure.Utility;
using Xunit;

public class AuthControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockJwtService = new Mock<IJwtService>();

        _mockUnitOfWork.Setup(u => u.UserRepository).Returns(_mockUserRepo.Object);
        _mockUnitOfWork.Setup(u => u.JwtService).Returns(_mockJwtService.Object);

        _controller = new AuthController(_mockUnitOfWork.Object);
    }

    [Fact]
    public void Login_ReturnsUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var request = new LoginRequest { Username = "user", Password = "pass" };
        _mockUserRepo.Setup(r => r.GetUserByUsername("user")).Returns((User?)null);

        // Act
        var result = _controller.Login(request);

        // Assert
        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, unauthorized.StatusCode);
    }

    [Fact]
    public void Login_ReturnsUnauthorized_WhenPasswordInvalid()
    {
        // Arrange
        var user = new User { Username = "user", PasswordHash = BCrypt.Net.BCrypt.HashPassword("otherpass") };
        var request = new LoginRequest { Username = "user", Password = "wrongpass" };
        _mockUserRepo.Setup(r => r.GetUserByUsername("user")).Returns(user);

        // Act
        var result = _controller.Login(request);

        // Assert
        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, unauthorized.StatusCode);
    }

    [Fact]
    public void Login_ReturnsToken_WhenCredentialsValid()
    {
        // Arrange
        var password = "pass";
        var user = new User { Username = "user", PasswordHash = BCrypt.Net.BCrypt.HashPassword(password) };
        var request = new LoginRequest { Username = "user", Password = password };
        _mockUserRepo.Setup(r => r.GetUserByUsername("user")).Returns(user);
        _mockJwtService.Setup(j => j.GenerateToken(user)).Returns("testtoken");

        // Act
        var result = _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        var data = Assert.IsType<Token>(okResult.Value);
        Assert.Equal("testtoken", data.token);
    }

    [Fact]
    public void Register_AddsUser_AndReturnsSuccess()
    {
        // Arrange
        User? addedUser = null;
        var request = new RegisterRequest { Username = "newuser", Password = "newpass", Role = "User" };
        _mockUserRepo.Setup(r => r.AddUser(It.IsAny<User>())).Callback<User>(u => addedUser = u);

        // Act
        var result = _controller.Register(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<RegisterResponse>(okResult.Value);
        Assert.Equal("User registered successfully", data.Message);
        Assert.NotNull(addedUser);
        Assert.Equal("newuser", addedUser!.Username);
        Assert.Equal("User", addedUser.Role);
        Assert.True(BCrypt.Net.BCrypt.Verify("newpass", addedUser.PasswordHash));
    }
}