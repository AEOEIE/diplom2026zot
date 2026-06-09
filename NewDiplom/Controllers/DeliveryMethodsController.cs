using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewDiplom.Data;
using NewDiplom.DTOs;
using NewDiplom.Entities;
using NewDiplom.Services; 

namespace NewDiplom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveryMethodsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuditService _auditService; 

    public DeliveryMethodsController(AppDbContext context, AuditService auditService) 
    {
        _context = context;
        _auditService = auditService; 
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        IQueryable<DeliveryMethod> query = _context.DeliveryMethods;

        if (role == "Client" || role == "Operator")
        {
            query = query.Where(x => x.IsActive);
        }

        var methods = await query.ToListAsync();

        return Ok(methods);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,ServiceManager")]
    public async Task<IActionResult> Create(CreateDeliveryMethodDto dto)
    {
        var method = new DeliveryMethod
        {
            Name = dto.Name,
            AdditionalPrice = dto.AdditionalPrice,
            EstimatedDaysMin = dto.EstimatedDaysMin,
            EstimatedDaysMax = dto.EstimatedDaysMax,
            IsActive = true
        };

        _context.DeliveryMethods.Add(method);
        await _context.SaveChangesAsync();
        await _auditService.LogAsync(
            actionType: "Create",
            tableName: "DeliveryMethods",
            recordId: method.Id.ToString(),
            newValues: AuditService.SerializeObject(new
            {
                dto.Name,
                dto.AdditionalPrice,
                dto.EstimatedDaysMin,
                dto.EstimatedDaysMax
            })
        );

        return Ok(method);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,ServiceManager")]
    public async Task<IActionResult> Update(int id, UpdateDeliveryMethodDto dto)
    {
        var method = await _context.DeliveryMethods.FindAsync(id);

        if (method == null)
            return NotFound();
        var oldValues = AuditService.SerializeObject(new
        {
            method.Name,
            method.AdditionalPrice,
            method.EstimatedDaysMin,
            method.EstimatedDaysMax,
            method.IsActive
        });

        method.Name = dto.Name;
        method.AdditionalPrice = dto.AdditionalPrice;
        method.EstimatedDaysMin = dto.EstimatedDaysMin;
        method.EstimatedDaysMax = dto.EstimatedDaysMax;
        method.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        await _auditService.LogAsync(
            actionType: "Update",
            tableName: "DeliveryMethods",
            recordId: id.ToString(),
            oldValues: oldValues,
            newValues: AuditService.SerializeObject(new
            {
                dto.Name,
                dto.AdditionalPrice,
                dto.EstimatedDaysMin,
                dto.EstimatedDaysMax,
                dto.IsActive
            })
        );

        return Ok(method);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var method = await _context.DeliveryMethods.FindAsync(id);

        if (method == null)
            return NotFound();
        var oldValues = AuditService.SerializeObject(new
        {
            method.Name,
            method.AdditionalPrice,
            method.EstimatedDaysMin,
            method.EstimatedDaysMax
        });

        _context.DeliveryMethods.Remove(method);
        await _context.SaveChangesAsync();
        await _auditService.LogAsync(
            actionType: "Delete",
            tableName: "DeliveryMethods",
            recordId: id.ToString(),
            oldValues: oldValues,
            newValues: "Удален"
        );

        return Ok();
    }
}