//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using NewDiplom.Data;
//using NewDiplom.DTOs;
//using NewDiplom.Entities;

//namespace NewDiplom.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//public class PostOfficesController : ControllerBase
//{
//    private readonly AppDbContext _context;

//    public PostOfficesController(AppDbContext context)
//    {
//        _context = context;
//    }

//    //[HttpGet]
//    //[Authorize]
//    //public async Task<IActionResult> GetAll()
//    //{
//    //    return Ok(await _context.PostOffices.ToListAsync());
//    //}
//    [HttpGet]
//    [Authorize]
//    public async Task<IActionResult> GetAll()
//    {
//        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

//        IQueryable<PostOffice> query =
//            _context.PostOffices;

//        if (role == "Client")
//        {
//            query = query.Where(x => x.IsActive);
//        }

//        var offices = await query.ToListAsync();

//        return Ok(await query.ToListAsync());
//    }

//    [HttpPost]
//    [Authorize(Roles = "Admin,Operator")]
//    public async Task<IActionResult> Create(CreatePostOfficeDto dto)
//    {
//        var office = new PostOffice
//        {
//            Code = dto.Code,
//            Name = dto.Name,
//            Address = dto.Address,
//            Phone = dto.Phone,
//            WorkingHours = dto.WorkingHours,
//            IsActive = true
//        };

//        _context.PostOffices.Add(office);

//        await _context.SaveChangesAsync();

//        return Ok(office);
//    }

//    [HttpPut("{id}")]
//    [Authorize(Roles = "Admin,Operator")]
//    public async Task<IActionResult> Update(int id, UpdatePostOfficeDto dto)
//    {
//        var office = await _context.PostOffices.FindAsync(id);

//        if (office == null)
//            return NotFound();

//        office.Code = dto.Code;
//        office.Name = dto.Name;
//        office.Address = dto.Address;
//        office.Phone = dto.Phone;
//        office.WorkingHours = dto.WorkingHours;
//        office.IsActive = dto.IsActive;

//        await _context.SaveChangesAsync();

//        return Ok(office);
//    }

//    [HttpDelete("{id}")]
//    [Authorize(Roles = "Admin")]
//    public async Task<IActionResult> Delete(int id)
//    {
//        var office = await _context.PostOffices.FindAsync(id);

//        if (office == null)
//            return NotFound();

//        _context.PostOffices.Remove(office);

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
public class PostOfficesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuditService _auditService; // ← ДОБАВИТЬ

    public PostOfficesController(AppDbContext context, AuditService auditService) // ← ДОБАВИТЬ auditService
    {
        _context = context;
        _auditService = auditService; // ← ДОБАВИТЬ
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        IQueryable<PostOffice> query = _context.PostOffices;

        if (role == "Client")
        {
            query = query.Where(x => x.IsActive);
        }

        var offices = await query.ToListAsync();

        return Ok(offices);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<IActionResult> Create(CreatePostOfficeDto dto)
    {
        var office = new PostOffice
        {
            Code = dto.Code,
            Name = dto.Name,
            Address = dto.Address,
            Phone = dto.Phone,
            WorkingHours = dto.WorkingHours,
            IsActive = true
        };

        _context.PostOffices.Add(office);
        await _context.SaveChangesAsync();

        // Логируем создание
        await _auditService.LogAsync(
            actionType: "Create",
            tableName: "PostOffices",
            recordId: office.Id.ToString(),
            newValues: AuditService.SerializeObject(new
            {
                dto.Code,
                dto.Name,
                dto.Address,
                dto.Phone,
                dto.WorkingHours
            })
        );

        return Ok(office);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<IActionResult> Update(int id, UpdatePostOfficeDto dto)
    {
        var office = await _context.PostOffices.FindAsync(id);

        if (office == null)
            return NotFound();

        // Сохраняем старые значения
        var oldValues = AuditService.SerializeObject(new
        {
            office.Code,
            office.Name,
            office.Address,
            office.Phone,
            office.WorkingHours,
            office.IsActive
        });

        office.Code = dto.Code;
        office.Name = dto.Name;
        office.Address = dto.Address;
        office.Phone = dto.Phone;
        office.WorkingHours = dto.WorkingHours;
        office.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        // Логируем изменение
        await _auditService.LogAsync(
            actionType: "Update",
            tableName: "PostOffices",
            recordId: id.ToString(),
            oldValues: oldValues,
            newValues: AuditService.SerializeObject(new
            {
                dto.Code,
                dto.Name,
                dto.Address,
                dto.Phone,
                dto.WorkingHours,
                dto.IsActive
            })
        );

        return Ok(office);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var office = await _context.PostOffices.FindAsync(id);

        if (office == null)
            return NotFound();

        // Сохраняем данные перед удалением
        var oldValues = AuditService.SerializeObject(new
        {
            office.Code,
            office.Name,
            office.Address,
            office.Phone,
            office.WorkingHours
        });

        _context.PostOffices.Remove(office);
        await _context.SaveChangesAsync();

        // Логируем удаление
        await _auditService.LogAsync(
            actionType: "Delete",
            tableName: "PostOffices",
            recordId: id.ToString(),
            oldValues: oldValues,
            newValues: "Удален"
        );

        return Ok();
    }
}