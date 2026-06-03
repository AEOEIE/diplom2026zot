// Tests/Services/AuditServiceTests.cs
using NUnit.Framework;
using Moq;
using Microsoft.EntityFrameworkCore;
using NewDiplom.Data;
using NewDiplom.Services;
using NewDiplom.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using FluentAssertions;

namespace NewDiplom.Tests;

[TestFixture]
public class AuditServiceTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
    [Test]
    public async Task LogAsync_ShouldAddAuditLog_WhenCalled()
    {
        var context = GetDbContext();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var service = new AuditService(context, httpContextAccessor.Object);
        await service.LogAsync("Create", "Users", "1", null, "{\"name\":\"test\"}");
        var logs = await context.AuditLogs.ToListAsync();
        logs.Should().HaveCount(1);
        logs[0].ActionType.Should().Be("Create");
        logs[0].TableName.Should().Be("Users");
        logs[0].RecordId.Should().Be("1");
    }
    [Test]
    public async Task LogAsync_ShouldSetUserId_WhenUserIsAuthenticated()
    {
        var context = GetDbContext();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "123")
        }));
        var httpContext = new DefaultHttpContext { User = user };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        var service = new AuditService(context, httpContextAccessor.Object);
        await service.LogAsync("Login", "Users", "123", null, "Success");
        var log = await context.AuditLogs.FirstAsync();
        log.UserId.Should().Be(123);
    }

    [Test]
    public void SerializeObject_ShouldReturnPrettyJson_WhenCalled()
    {
        var obj = new { Name = "Test", Value = 123 };
        var json = AuditService.SerializeObject(obj);
        json.Should().Contain("Test");
        json.Should().Contain("123");
        json.Should().Contain("\n");
    }
}