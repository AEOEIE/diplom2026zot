// Tests/Controllers/AuthControllerTests.cs - ИСПРАВЛЕННЫЙ
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NewDiplom.Controllers;
using NewDiplom.Data;
using NewDiplom.DTOs.Auth;
using NewDiplom.Entities;
using NewDiplom.Services;
using NUnit.Framework;
using System.Security.Claims;

namespace NewDiplom.Tests;

[TestFixture]
public class AuthControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private AuthController CreateController(AppDbContext context, int userId = 1, string role = "Admin")
    {
        var jwtService = new Mock<JwtService>(null);

        // ТОЛЬКО 2 ПАРАМЕТРА: context и jwtService
        var controller = new AuthController(context, jwtService.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        }));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        return controller;
    }

    [Test]
    public async Task Register_ShouldCreateUser_WhenValidRequest()
    {
        // Arrange
        var context = GetDbContext();
        var controller = CreateController(context);

        context.Roles.Add(new Role { Id = 1, Name = "Client" });
        await context.SaveChangesAsync();

        var request = new RegisterRequest
        {
            Login = "testuser",
            Email = "test@test.com",
            Password = "Password123!"
        };

        // Act
        var result = await controller.Register(request);

        // Assert
        var users = await context.Users.ToListAsync();
        users.Should().HaveCount(1);
        users[0].Login.Should().Be("testuser");
        users[0].Email.Should().Be("test@test.com");
    }

    [Test]
    public async Task Register_ShouldReturnError_WhenLoginExists()
    {
        // Arrange
        var context = GetDbContext();
        var controller = CreateController(context);

        var role = new Role { Id = 1, Name = "Client" };
        context.Roles.Add(role);
        context.Users.Add(new User
        {
            Login = "existing",
            Email = "existing@test.com",
            PasswordHash = "hash",
            RoleId = 1,
            CreatedAt = DateTime.UtcNow,
            LockoutEnabled = true
        });
        await context.SaveChangesAsync();

        var request = new RegisterRequest
        {
            Login = "existing",
            Email = "new@test.com",
            Password = "Password123!"
        };

        // Act
        var result = await controller.Register(request);

        // Assert
        var users = await context.Users.CountAsync();
        users.Should().Be(1); // Пользователь не добавился
    }


    [Test]
    public async Task Login_ShouldReturnUnauthorized_WhenInvalidPassword()
    {
        // Arrange
        var context = GetDbContext();
        var controller = CreateController(context);

        var role = new Role { Id = 1, Name = "Client" };
        context.Roles.Add(role);
        context.Users.Add(new User
        {
            Id = 1,
            Login = "testuser",
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            RoleId = 1,
            CreatedAt = DateTime.UtcNow,
            LockoutEnabled = true,
            AccessFailedCount = 0
        });
        await context.SaveChangesAsync();

        var request = new LoginRequest
        {
            Login = "testuser",
            Password = "WrongPassword!"
        };

        // Act
        var result = await controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Test]
    public async Task Login_ShouldBlockUser_After10FailedAttempts()
    {
        // Arrange
        var context = GetDbContext();
        var controller = CreateController(context);

        var role = new Role { Id = 1, Name = "Client" };
        context.Roles.Add(role);
        var user = new User
        {
            Id = 1,
            Login = "testuser",
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            RoleId = 1,
            CreatedAt = DateTime.UtcNow,
            LockoutEnabled = true,
            AccessFailedCount = 9 // Уже 9 неудачных попыток
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var request = new LoginRequest
        {
            Login = "testuser",
            Password = "WrongPassword!"
        };

        // Act
        var result = await controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();

        var updatedUser = await context.Users.FirstAsync();
        updatedUser.AccessFailedCount.Should().Be(10);
        updatedUser.LockoutEnd.Should().NotBeNull();
    }

    [Test]
    public async Task Login_ShouldNotIncreaseCounter_WhenUserIsLocked()
    {
        // Arrange
        var context = GetDbContext();
        var controller = CreateController(context);

        var role = new Role { Id = 1, Name = "Client" };
        context.Roles.Add(role);
        var user = new User
        {
            Id = 1,
            Login = "testuser",
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            RoleId = 1,
            CreatedAt = DateTime.UtcNow,
            LockoutEnabled = true,
            AccessFailedCount = 10,
            LockoutEnd = DateTime.Now.AddMinutes(5) // Заблокирован
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var request = new LoginRequest
        {
            Login = "testuser",
            Password = "WrongPassword!"
        };

        // Act
        var result = await controller.Login(request);

        // Assert
        var updatedUser = await context.Users.FirstAsync();
        updatedUser.AccessFailedCount.Should().Be(10); // Не увеличился
    }

   
}