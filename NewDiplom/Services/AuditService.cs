using Microsoft.EntityFrameworkCore;
using NewDiplom.Data;
using NewDiplom.Entities;
using System.Text.Json;
using System.Security.Claims;

namespace NewDiplom.Services;

public class AuditService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(
        string actionType,
        string tableName,
        string? recordId = null,
        string? oldValues = null,
        string? newValues = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var auditLog = new AuditLog
        {
            UserId = GetUserId(httpContext),
            ActionType = actionType,
            TableName = tableName,
            RecordId = recordId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = httpContext?.Connection.RemoteIpAddress?.ToString(),
            UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
            CreatedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }

    private int? GetUserId(HttpContext? httpContext)
    {
        if (httpContext?.User == null) return null;

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }

    public static string SerializeObject(object obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            WriteIndented = true,
            MaxDepth = 3
        });
    }
}