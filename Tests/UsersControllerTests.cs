// Tests/Controllers/UsersControllerTests.cs
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NewDiplom.Controllers;
using NewDiplom.Data;
using NewDiplom.DTOs.Users;
using NewDiplom.Entities;
using NewDiplom.Services;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace NewDiplom.Tests;

[TestFixture]
public class UsersControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private UsersController CreateController(AppDbContext context, int userId = 1, string role = "Admin")
    {
        var auditService = new Mock<AuditService>(context, Mock.Of<IHttpContextAccessor>());
        var controller = new UsersController(context, auditService.Object);

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
    public async Task GetAll_ShouldReturnUsers_WhenAdmin()
    {
        var context = GetDbContext();
        var controller = CreateController(context, role: "Admin");
        var role = new Role { Id = 1, Name = "Client" };
        context.Roles.Add(role);
        context.Users.Add(new User
        {
            Login = "user1",
            Email = "user1@test.com",
            PasswordHash = "hash",
            RoleId = 1,
            CreatedAt = DateTime.UtcNow
        });
        context.Users.Add(new User
        {
            Login = "user2",
            Email = "user2@test.com",
            PasswordHash = "hash",
            RoleId = 1,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
        var result = await controller.GetAll();
        var okResult = result as OkObjectResult;
        var users = okResult!.Value as IEnumerable<UserDto>;
        users.Should().HaveCount(2);
    }

    [Test]
    public async Task CreateEmployee_ShouldAddUser_WhenValidRequest()
    {
        var context = GetDbContext();
        var controller = CreateController(context);
        var role = new Role { Id = 1, Name = "Operator" };
        context.Roles.Add(role);
        await context.SaveChangesAsync();
        var request = new CreateEmployeeRequest
        {
            Login = "newemployee",
            Email = "emp@test.com",
            Password = "Password123!",
            FirstName = "Иван",
            LastName = "Иванов",
            Phone = "89001234567",
            RoleId = 1
        };
        var result = await controller.CreateEmployee(request);
        var users = await context.Users.ToListAsync();
        users.Should().HaveCount(1);
        users[0].Login.Should().Be("newemployee");
    }
    [Test]
    public async Task UpdateEmployee_ShouldModifyUser_WhenValidRequest()
    {
        var context = GetDbContext();
        var controller = CreateController(context);
        var role = new Role { Id = 1, Name = "Operator" };
        context.Roles.Add(role);
        var user = new User
        {
            Id = 1,
            Login = "oldlogin",
            Email = "old@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("OldPass123!"),
            RoleId = 1,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var request = new UpdateEmployeeRequest
        {
            Login = "newlogin",
            Email = "new@test.com",
            FirstName = "Петр",
            LastName = "Петров",
            Phone = "89009998877",
            RoleId = 1,
            IsActive = false
        };
        var result = await controller.UpdateEmployee(1, request);
        var updatedUser = await context.Users.FirstAsync();
        updatedUser.Login.Should().Be("newlogin");
        updatedUser.FirstName.Should().Be("Петр");
        updatedUser.IsActive.Should().BeFalse();
    }

    [Test]
    public async Task UpdateEmployee_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var context = GetDbContext();
        var controller = CreateController(context);
        var role = new Role { Id = 1, Name = "Operator" };
        context.Roles.Add(role);
        await context.SaveChangesAsync();
        var request = new UpdateEmployeeRequest
        {
            Login = "newlogin",
            Email = "new@test.com",
            FirstName = "Петр",
            LastName = "Петров",
            RoleId = 1,
            IsActive = true
        };
        var result = await controller.UpdateEmployee(999, request);
        result.Should().BeOfType<NotFoundResult>();
    }
}