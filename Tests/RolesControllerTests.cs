using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NewDiplom.Controllers;
using NewDiplom.Data;
using NewDiplom.DTOs;
using NewDiplom.Entities;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace NewDiplom.Tests;

[TestFixture]
public class RolesControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private RolesController CreateController(AppDbContext context)
    {
        var controller = new RolesController(context);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Admin")
        }));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        return controller;
    }

    [Test]
    public async Task GetAll_ShouldReturnAllRoles()
    {
        var context = GetDbContext();
        context.Roles.AddRange(new[]
        {
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Operator" },
            new Role { Id = 3, Name = "Client" },
            new Role { Id = 4, Name = "ServiceManager" },
            new Role { Id = 5, Name = "DepartmentHead" }
        });
        await context.SaveChangesAsync();
        var controller = CreateController(context);
        var result = await controller.GetAll();
        var okResult = result as OkObjectResult;
        var roles = okResult!.Value as IEnumerable<RoleDto>;
        roles.Should().HaveCount(5);
    }

    [Test]
    public async Task GetAll_ShouldReturnCorrectDtoProperties()
    {
        var context = GetDbContext();
        context.Roles.Add(new Role { Id = 1, Name = "Admin" });
        await context.SaveChangesAsync();
        var controller = CreateController(context);
        var result = await controller.GetAll();
        var okResult = result as OkObjectResult;
        var roles = okResult!.Value as IEnumerable<RoleDto>;
        var role = roles!.First();

        role.Id.Should().Be(1);
        role.Name.Should().Be("Admin");
    }

    [Test]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoRoles()
    {
        var context = GetDbContext();
        var controller = CreateController(context);
        var result = await controller.GetAll();
        var okResult = result as OkObjectResult;
        var roles = okResult!.Value as IEnumerable<RoleDto>;
        roles.Should().BeEmpty();
    }
}