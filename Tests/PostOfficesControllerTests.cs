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
public class PostOfficesControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
    private PostOfficesController CreateController(AppDbContext context, string role = "Admin")
    {
        var auditService = new Mock<AuditService>(context, Mock.Of<IHttpContextAccessor>());
        var controller = new PostOfficesController(context, auditService.Object);
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
        context.PostOffices.AddRange(new[]
        {
            new PostOffice { Id = 1, Name = "Active1", Code = "001", IsActive = true },
            new PostOffice { Id = 2, Name = "Active2", Code = "002", IsActive = true },
            new PostOffice { Id = 3, Name = "Inactive", Code = "003", IsActive = false }
        });
        await context.SaveChangesAsync();
        var controller = CreateController(context, role: "Client");
        var result = await controller.GetAll();
        var okResult = result as OkObjectResult;
        var offices = okResult!.Value as IEnumerable<PostOffice>;
        offices.Should().HaveCount(2);
        offices.Should().NotContain(x => x.Name == "Inactive");
    }
    [Test]
    public async Task GetAll_ShouldReturnAll_WhenAdmin()
    {
        var context = GetDbContext();
        context.PostOffices.AddRange(new[]
        {
            new PostOffice { Id = 1, Name = "Active1", Code = "001", IsActive = true },
            new PostOffice { Id = 2, Name = "Active2", Code = "002", IsActive = true },
            new PostOffice { Id = 3, Name = "Inactive", Code = "003", IsActive = false }
        });
        await context.SaveChangesAsync();
        var controller = CreateController(context, role: "Admin");
        var result = await controller.GetAll();
        var okResult = result as OkObjectResult;
        var offices = okResult!.Value as IEnumerable<PostOffice>;
        offices.Should().HaveCount(3);
    }
    [Test]
    public async Task Create_ShouldAddOffice_WhenValidRequest()
    {
        var context = GetDbContext();
        var controller = CreateController(context, role: "Admin");
        var dto = new CreatePostOfficeDto
        {
            Code = "001",
            Name = "Центральное отделение",
            Address = "ул. Центральная, д.1",
            Phone = "88001234567",
            WorkingHours = "9:00-20:00"
        };
        var result = await controller.Create(dto);
        var offices = await context.PostOffices.ToListAsync();
        offices.Should().HaveCount(1);
        offices[0].Name.Should().Be("Центральное отделение");
        offices[0].Code.Should().Be("001");
        offices[0].IsActive.Should().BeTrue();
    }
    [Test]
    public async Task Update_ShouldModifyOffice_WhenValidRequest()
    {
        var context = GetDbContext();
        var office = new PostOffice
        {
            Id = 1,
            Code = "001",
            Name = "Старое название",
            Address = "Старый адрес",
            Phone = "111111111",
            WorkingHours = "8:00-17:00",
            IsActive = true
        };
        context.PostOffices.Add(office);
        await context.SaveChangesAsync();
        var controller = CreateController(context, role: "Admin");
        var dto = new UpdatePostOfficeDto
        {
            Code = "002",
            Name = "Новое название",
            Address = "Новый адрес",
            Phone = "222222222",
            WorkingHours = "10:00-19:00",
            IsActive = false
        };
        var result = await controller.Update(1, dto);
        var updatedOffice = await context.PostOffices.FirstAsync();
        updatedOffice.Name.Should().Be("Новое название");
        updatedOffice.Code.Should().Be("002");
        updatedOffice.IsActive.Should().BeFalse();
    }
    [Test]
    public async Task Update_ShouldReturnNotFound_WhenOfficeDoesNotExist()
    {
        var context = GetDbContext();
        var controller = CreateController(context, role: "Admin");
        var dto = new UpdatePostOfficeDto
        {
            Code = "002",
            Name = "Новое название",
            Address = "Новый адрес",
            Phone = "222222222",
            WorkingHours = "10:00-19:00",
            IsActive = true
        };
        var result = await controller.Update(999, dto);
        result.Should().BeOfType<NotFoundResult>();
    }
    [Test]
    public async Task Delete_ShouldRemoveOffice_WhenExists()
    {
        var context = GetDbContext();
        var office = new PostOffice
        {
            Id = 1,
            Code = "001",
            Name = "Тестовое отделение",
            Address = "Тестовый адрес",
            Phone = "123456789",
            WorkingHours = "9:00-18:00",
            IsActive = true
        };
        context.PostOffices.Add(office);
        await context.SaveChangesAsync();
        var controller = CreateController(context, role: "Admin");
        var result = await controller.Delete(1);
        var offices = await context.PostOffices.ToListAsync();
        offices.Should().HaveCount(0);
    }

    [Test]
    public async Task Delete_ShouldReturnNotFound_WhenOfficeDoesNotExist()
    {
        var context = GetDbContext();
        var controller = CreateController(context, role: "Admin");
        var result = await controller.Delete(999);
        result.Should().BeOfType<NotFoundResult>();
    }
}