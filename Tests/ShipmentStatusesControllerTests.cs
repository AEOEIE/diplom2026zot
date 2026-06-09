using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NewDiplom.Controllers;
using NewDiplom.Data;
using NewDiplom.DTOs.Shipments;
using NewDiplom.Entities;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace NewDiplom.Tests;

[TestFixture]
public class ShipmentStatusesControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private ShipmentStatusesController CreateController(AppDbContext context)
    {
        var controller = new ShipmentStatusesController(context);

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
    public async Task GetAll_ShouldReturnAllStatuses()
    {
        var context = GetDbContext();
        context.ShipmentStatuses.AddRange(new[]
        {
            new ShipmentStatus { Id = 1, Name = "Принято", IsFinal = false },
            new ShipmentStatus { Id = 2, Name = "В пути", IsFinal = false },
            new ShipmentStatus { Id = 3, Name = "Доставлено", IsFinal = true }
        });
        await context.SaveChangesAsync();
        var controller = CreateController(context);
        var result = await controller.GetAll();
        var okResult = result as OkObjectResult;
        var statuses = okResult!.Value as IEnumerable<ShipmentStatusDto>;
        statuses.Should().HaveCount(3);
    }
    [Test]
    public async Task GetAll_ShouldReturnCorrectDtoProperties()
    {
        var context = GetDbContext();
        context.ShipmentStatuses.Add(new ShipmentStatus
        {
            Id = 1,
            Name = "Принято",
            IsFinal = false
        });
        await context.SaveChangesAsync();
        var controller = CreateController(context);
        var result = await controller.GetAll();
        var okResult = result as OkObjectResult;
        var statuses = okResult!.Value as IEnumerable<ShipmentStatusDto>;
        var status = statuses!.First();
        status.Id.Should().Be(1);
        status.Name.Should().Be("Принято");
        status.IsFinal.Should().BeFalse();
    }

    [Test]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoStatuses()
    {
        var context = GetDbContext();
        var controller = CreateController(context);
        var result = await controller.GetAll();
        var okResult = result as OkObjectResult;
        var statuses = okResult!.Value as IEnumerable<ShipmentStatusDto>;
        statuses.Should().BeEmpty();
    }
}