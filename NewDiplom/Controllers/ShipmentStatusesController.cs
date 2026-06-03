using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewDiplom.DTOs.Shipments;
using NewDiplom.Data;

namespace NewDiplom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentStatusesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ShipmentStatusesController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var statuses = await _context.ShipmentStatuses
            .Select(s => new ShipmentStatusDto
            {
                Id = s.Id,
                Name = s.Name,
                IsFinal = s.IsFinal
            })
            .ToListAsync();

        return Ok(statuses);
    }

}