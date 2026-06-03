// Tests/Controllers/ShipmentsControllerTests.cs
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NewDiplom.Controllers;
using NewDiplom.Data;
using NewDiplom.DTOs.Shipments;
using NewDiplom.Entities;
using NewDiplom.Services;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace NewDiplom.Tests;

[TestFixture]
public class ShipmentsControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private ShipmentsController CreateController(AppDbContext context, int userId = 1, string role = "Client")
    {
        var auditService = new Mock<AuditService>(context, Mock.Of<IHttpContextAccessor>());
        var controller = new ShipmentsController(context, auditService.Object);

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

    private async Task SeedData(AppDbContext context)
    {
        context.ShipmentStatuses.AddRange(new[]
        {
            new ShipmentStatus { Id = 1, Name = "Принято" },
            new ShipmentStatus { Id = 2, Name = "В пути" },
            new ShipmentStatus { Id = 3, Name = "Доставлено" }
        });

        context.ServiceTypes.Add(new ServiceType
        {
            Id = 1,
            Name = "Стандарт",
            BasePrice = 500,
            IsActive = true
        });

        context.DeliveryMethods.Add(new DeliveryMethod
        {
            Id = 1,
            Name = "Курьер",
            AdditionalPrice = 300,
            IsActive = true
        });

        context.PostOffices.Add(new PostOffice
        {
            Id = 1,
            Name = "Центральное",
            Code = "001",
            IsActive = true
        });

        await context.SaveChangesAsync();
    }

    [Test]
    public async Task Create_ShouldAddShipment_WhenValidRequest()
    {
        var context = GetDbContext();
        await SeedData(context);
        var controller = CreateController(context, userId: 1, role: "Client");
        var request = new CreateShipmentRequest
        {
            RecipientName = "Тестовый Получатель",
            RecipientPhone = "89001234567",
            CurrentOfficeId = 1,
            DestinationOfficeId = 1,
            ServiceTypeId = 1,
            DeliveryMethodId = 1,
            WeightKg = 2.5m,
            DeclaredValue = 1000,
            Notes = "Тестовое отправление"
        };
        var result = await controller.Create(request);
        var shipments = await context.Shipments.ToListAsync();
        shipments.Should().HaveCount(1);
        shipments[0].RecipientName.Should().Be("Тестовый Получатель");
        shipments[0].WeightKg.Should().Be(2.5m);
    }

    [Test]
    public async Task Create_ShouldReturnBadRequest_WhenRecipientNameMissing()
    {
        var context = GetDbContext();
        await SeedData(context);
        var controller = CreateController(context, userId: 1, role: "Client");
        var request = new CreateShipmentRequest
        {
            RecipientName = "",
            RecipientPhone = "89001234567",
            CurrentOfficeId = 1,
            DestinationOfficeId = 1,
            ServiceTypeId = 1,
            DeliveryMethodId = 1,
            WeightKg = 2.5m
        };
        var result = await controller.Create(request);
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test]
    public async Task UpdateStatus_ShouldChangeStatus_WhenValid()
    {
        var context = GetDbContext();
        await SeedData(context);

        var shipment = new Shipment
        {
            TrackingNumber = "TRACK123",
            ClientId = 1,
            RecipientName = "Тест",
            CurrentStatusId = 1,
            ServiceTypeId = 1,
            DeliveryMethodId = 1,
            CurrentOfficeId = 1,
            DestinationOfficeId = 1,
            WeightKg = 1,
            TotalPrice = 500,
            AcceptedAt = DateTime.UtcNow
        };
        context.Shipments.Add(shipment);
        await context.SaveChangesAsync();

        var controller = CreateController(context, userId: 1, role: "Admin");

        var request = new UpdateShipmentStatusRequest
        {
            StatusId = 2
        };
        var result = await controller.UpdateStatus(shipment.Id, request);
        var updatedShipment = await context.Shipments.FirstAsync();
        updatedShipment.CurrentStatusId.Should().Be(2);
    }

    [Test]
    public async Task GetById_ShouldReturnShipment_WhenUserIsOwner()
    {
        var context = GetDbContext();
        await SeedData(context);

        var shipment = new Shipment
        {
            TrackingNumber = "TRACK456",
            ClientId = 1,
            RecipientName = "Тест",
            CurrentStatusId = 1,
            ServiceTypeId = 1,
            DeliveryMethodId = 1,
            CurrentOfficeId = 1,
            DestinationOfficeId = 1,
            WeightKg = 1,
            TotalPrice = 500,
            AcceptedAt = DateTime.UtcNow
        };
        context.Shipments.Add(shipment);
        await context.SaveChangesAsync();
        var controller = CreateController(context, userId: 1, role: "Client");
        var result = await controller.GetById(shipment.Id);
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Test]
    public async Task GetById_ShouldReturnForbidden_WhenUserIsNotOwner()
    {
        var context = GetDbContext();
        await SeedData(context);
        var shipment = new Shipment
        {
            TrackingNumber = "TRACK456",
            ClientId = 2, // Другой пользователь
            RecipientName = "Тест",
            CurrentStatusId = 1,
            ServiceTypeId = 1,
            DeliveryMethodId = 1,
            CurrentOfficeId = 1,
            DestinationOfficeId = 1,
            WeightKg = 1,
            TotalPrice = 500,
            AcceptedAt = DateTime.UtcNow
        };
        context.Shipments.Add(shipment);
        await context.SaveChangesAsync();
        var controller = CreateController(context, userId: 1, role: "Client");
        var result = await controller.GetById(shipment.Id);
        result.Should().BeOfType<ForbidResult>();
    }
}