//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using NewDiplom.Data;
//using NewDiplom.DTOs;
//using NewDiplom.Entities;

//namespace NewDiplom.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//public class DeliveryMethodsController : ControllerBase
//{
//    private readonly AppDbContext _context;

//    public DeliveryMethodsController(AppDbContext context)
//    {
//        _context = context;
//    }

//    //[HttpGet]
//    //[Authorize]
//    //public async Task<IActionResult> GetAll()
//    //{
//    //    return Ok(await _context.DeliveryMethods.ToListAsync());
//    //}
//    [HttpGet]
//    [Authorize]
//    public async Task<IActionResult> GetAll()
//    {
//        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

//        IQueryable<DeliveryMethod> query =
//            _context.DeliveryMethods;

//        if (role == "Client" || role == "Operator")
//        {
//            query = query.Where(x => x.IsActive);
//        }

//        var methods = await query.ToListAsync();

//        return Ok(methods);
//    }

//    [HttpPost]
//    [Authorize(Roles = "Admin,ServiceManager")]
//    public async Task<IActionResult> Create(CreateDeliveryMethodDto dto)
//    {
//        var method = new DeliveryMethod
//        {
//            Name = dto.Name,
//            AdditionalPrice = dto.AdditionalPrice,
//            EstimatedDaysMin = dto.EstimatedDaysMin,
//            EstimatedDaysMax = dto.EstimatedDaysMax,
//            IsActive = true
//        };

//        _context.DeliveryMethods.Add(method);

//        await _context.SaveChangesAsync();

//        return Ok(method);
//    }

//    [HttpPut("{id}")]
//    [Authorize(Roles = "Admin,ServiceManager")]
//    public async Task<IActionResult> Update(int id, UpdateDeliveryMethodDto dto)
//    {
//        var method = await _context.DeliveryMethods.FindAsync(id);

//        if (method == null)
//            return NotFound();

//        method.Name = dto.Name;
//        method.AdditionalPrice = dto.AdditionalPrice;
//        method.EstimatedDaysMin = dto.EstimatedDaysMin;
//        method.EstimatedDaysMax = dto.EstimatedDaysMax;
//        method.IsActive = dto.IsActive;

//        await _context.SaveChangesAsync();

//        return Ok(method);
//    }

//    [HttpDelete("{id}")]
//    [Authorize(Roles = "Admin")]
//    public async Task<IActionResult> Delete(int id)
//    {
//        var method = await _context.DeliveryMethods.FindAsync(id);

//        if (method == null)
//            return NotFound();

//        _context.DeliveryMethods.Remove(method);

//        await _context.SaveChangesAsync();

//        return Ok();
//    }
//}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewDiplom.Data;
using NewDiplom.DTOs;
using NewDiplom.Entities;
using NewDiplom.Services; // ← ДОБАВИТЬ

namespace NewDiplom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveryMethodsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuditService _auditService; // ← ДОБАВИТЬ

    public DeliveryMethodsController(AppDbContext context, AuditService auditService) // ← ДОБАВИТЬ auditService
    {
        _context = context;
        _auditService = auditService; // ← ДОБАВИТЬ
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

        // Логируем создание
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

        // Сохраняем старые значения
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

        // Логируем изменение
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

        // Сохраняем данные перед удалением
        var oldValues = AuditService.SerializeObject(new
        {
            method.Name,
            method.AdditionalPrice,
            method.EstimatedDaysMin,
            method.EstimatedDaysMax
        });

        _context.DeliveryMethods.Remove(method);
        await _context.SaveChangesAsync();

        // Логируем удаление
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