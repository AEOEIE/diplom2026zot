using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NewDiplom.Controllers;
using NewDiplom.Data;
using NewDiplom.DTOs;
using NewDiplom.Entities;
using NewDiplom.Services;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace NewDiplom.Tests;

[TestFixture]
public class DeliveryMethodsControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private DeliveryMethodsController CreateController(AppDbContext context, string role = "Admin")
    {
        var auditService = new Mock<AuditService>(context, Mock.Of<IHttpContextAccessor>());
        var controller = new DeliveryMethodsController(context, auditService.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, role)
        }));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        return controller;
    }

    [Test]
    public async Task GetAll_ShouldReturnOnlyActive_WhenClient()
    {
        var context = GetDbContext();
        context.DeliveryMethods.AddRange(new[]
        {
            new DeliveryMethod { Id = 1, Name = "Active1", IsActive = true },
            new DeliveryMethod { Id = 2, Name = "Active2", IsActive = true },
            new DeliveryMethod { Id = 3, Name = "Inactive", IsActive = false }
        });
        await context.SaveChangesAsync();

        var controller = CreateController(context, role: "Client");
        var result = await controller.GetAll();
        var okResult = result as OkObjectResult;
        var methods = okResult!.Value as IEnumerable<DeliveryMethod>;
        methods.Should().HaveCount(2);
        methods.Should().NotContain(x => x.Name == "Inactive");
    }

    [Test]
    public async Task GetAll_ShouldReturnAll_WhenAdmin()
    {
        var context = GetDbContext();
        context.DeliveryMethods.AddRange(new[]
        {
            new DeliveryMethod { Id = 1, Name = "Active1", IsActive = true },
            new DeliveryMethod { Id = 2, Name = "Active2", IsActive = true },
            new DeliveryMethod { Id = 3, Name = "Inactive", IsActive = false }
        });
        await context.SaveChangesAsync();
        var controller = CreateController(context, role: "Admin");
        var result = await controller.GetAll();
        var okResult = result as OkObjectResult;
        var methods = okResult!.Value as IEnumerable<DeliveryMethod>;
        methods.Should().HaveCount(3);
    }

    [Test]
    public async Task Create_ShouldAddMethod_WhenValidRequest()
    {
        var context = GetDbContext();
        var controller = CreateController(context, role: "Admin");
        var dto = new CreateDeliveryMethodDto
        {
            Name = "Новый способ",
            AdditionalPrice = 200,
            EstimatedDaysMin = 1,
            EstimatedDaysMax = 3
        };
        var result = await controller.Create(dto);
        var methods = await context.DeliveryMethods.ToListAsync();
        methods.Should().HaveCount(1);
        methods[0].Name.Should().Be("Новый способ");
        methods[0].IsActive.Should().BeTrue();
    }

    [Test]
    public async Task Update_ShouldModifyMethod_WhenValidRequest()
    {
        var context = GetDbContext();
        var method = new DeliveryMethod
        {
            Id = 1,
            Name = "Старое имя",
            AdditionalPrice = 100,
            IsActive = true
        };
        context.DeliveryMethods.Add(method);
        await context.SaveChangesAsync();
        var controller = CreateController(context, role: "Admin");
        var dto = new UpdateDeliveryMethodDto
        {
            Name = "Новое имя",
            AdditionalPrice = 300,
            EstimatedDaysMin = 2,
            EstimatedDaysMax = 5,
            IsActive = false
        };
        var result = await controller.Update(1, dto);
        var updatedMethod = await context.DeliveryMethods.FirstAsync();
        updatedMethod.Name.Should().Be("Новое имя");
        updatedMethod.AdditionalPrice.Should().Be(300);
        updatedMethod.IsActive.Should().BeFalse();
    }
}