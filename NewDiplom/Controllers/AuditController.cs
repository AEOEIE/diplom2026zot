using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewDiplom.Data;

namespace NewDiplom.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AuditController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuditController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? actionType = null,
        [FromQuery] string? tableName = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var query = _context.AuditLogs
            .Include(x => x.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(actionType))
            query = query.Where(x => x.ActionType == actionType);

        if (!string.IsNullOrEmpty(tableName))
            query = query.Where(x => x.TableName == tableName);

        if (from.HasValue)
            query = query.Where(x => x.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.CreatedAt <= to.Value);

        var total = await query.CountAsync();

        var logs = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.Id,
                x.ActionType,
                x.TableName,
                x.RecordId,
                x.OldValues,
                x.NewValues,
                x.CreatedAt,
                x.IpAddress,
                User = x.User != null ? new { x.User.Login, x.User.Id } : null
            })
            .ToListAsync();

        return Ok(new
        {
            items = logs,
            total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling((double)total / pageSize)
        });
    }

    [HttpGet("actions")]
    public async Task<IActionResult> GetActionTypes()
    {
        var actions = await _context.AuditLogs
            .Select(x => x.ActionType)
            .Distinct()
            .ToListAsync();

        return Ok(actions);
    }

    [HttpGet("tables")]
    public async Task<IActionResult> GetTableNames()
    {
        var tables = await _context.AuditLogs
            .Select(x => x.TableName)
            .Distinct()
            .ToListAsync();

        return Ok(tables);
    }
}