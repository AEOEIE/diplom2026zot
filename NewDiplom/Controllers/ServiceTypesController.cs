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
public class ServiceTypesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuditService _auditService; 

    public ServiceTypesController(AppDbContext context, AuditService auditService) 
    {
        _context = context;
        _auditService = auditService; 
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        IQueryable<ServiceType> query = _context.ServiceTypes;

        if (role == "Client" || role == "Operator")
        {
            query = query.Where(x => x.IsActive);
        }

        var services = await query.ToListAsync();

        return Ok(services);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,ServiceManager")]
    public async Task<IActionResult> Create(CreateServiceTypeDto dto)
    {
        var service = new ServiceType
        {
            Name = dto.Name,
            BasePrice = dto.BasePrice,
            Description = dto.Description,
            IsActive = true
        };

        _context.ServiceTypes.Add(service);
        await _context.SaveChangesAsync();
        await _auditService.LogAsync(
            actionType: "Create",
            tableName: "ServiceTypes",
            recordId: service.Id.ToString(),
            newValues: AuditService.SerializeObject(new
            {
                dto.Name,
                dto.BasePrice,
                dto.Description
            })
        );

        return Ok(service);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,ServiceManager")]
    public async Task<IActionResult> Update(int id, UpdateServiceTypeDto dto)
    {
        var service = await _context.ServiceTypes.FindAsync(id);

        if (service == null)
            return NotFound();
        var oldValues = AuditService.SerializeObject(new
        {
            service.Name,
            service.BasePrice,
            service.Description,
            service.IsActive
        });

        service.Name = dto.Name;
        service.BasePrice = dto.BasePrice;
        service.Description = dto.Description;
        service.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        // Логируем изменение
        await _auditService.LogAsync(
            actionType: "Update",
            tableName: "ServiceTypes",
            recordId: id.ToString(),
            oldValues: oldValues,
            newValues: AuditService.SerializeObject(new
            {
                dto.Name,
                dto.BasePrice,
                dto.Description,
                dto.IsActive
            })
        );

        return Ok(service);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var service = await _context.ServiceTypes.FindAsync(id);

        if (service == null)
            return NotFound();

        // Сохраняем данные перед удалением
        var oldValues = AuditService.SerializeObject(new
        {
            service.Name,
            service.BasePrice,
            service.Description
        });

        _context.ServiceTypes.Remove(service);
        await _context.SaveChangesAsync();

        // Логируем удаление
        await _auditService.LogAsync(
            actionType: "Delete",
            tableName: "ServiceTypes",
            recordId: id.ToString(),
            oldValues: oldValues,
            newValues: "Удален"
        );

        return Ok();
    }
}