using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewDiplom.Data;
using NewDiplom.DTOs.Users;
using NewDiplom.Entities;
using NewDiplom.Services; 

namespace NewDiplom.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Operator")]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuditService _auditService; 

    public ClientsController(AppDbContext context, AuditService auditService) 
    {
        _context = context;
        _auditService = auditService; 
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.Users
            .Include(x => x.Role)
            .Include(x => x.PostOffice)
            .Where(x => x.Role.Name == "Client")
            .ToListAsync();

        var result = users.Select(user => new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            Email = user.Email,
            Login = user.Login,
            Phone = user.Phone,
            Role = user.Role.Name,
            RoleId = user.RoleId,
            PostOfficeId = user.PostOfficeId,
            PostOfficeName = user.PostOffice != null ? user.PostOffice.Name : null,
            PassportSeries = user.PassportSeries,
            PassportNumber = user.PassportNumber,
            PassportIssuedBy = user.PassportIssuedBy,
            PassportIssueDate = user.PassportIssueDate,
            RegistrationAddress = user.RegistrationAddress,
            IsActive = user.IsActive,
            IsProfileCompleted = !string.IsNullOrWhiteSpace(user.FirstName) &&
                !string.IsNullOrWhiteSpace(user.LastName) &&
                !string.IsNullOrWhiteSpace(user.Phone) &&
                !string.IsNullOrWhiteSpace(user.Email) &&
                !string.IsNullOrWhiteSpace(user.PassportSeries) &&
                !string.IsNullOrWhiteSpace(user.PassportNumber) &&
                !string.IsNullOrWhiteSpace(user.PassportIssuedBy) &&
                user.PassportIssueDate != null &&
                !string.IsNullOrWhiteSpace(user.RegistrationAddress)
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _context.Users
            .Include(x => x.Role)
            .Include(x => x.PostOffice)
            .FirstOrDefaultAsync(x => x.Id == id && x.Role.Name == "Client");

        if (user == null)
            return NotFound();

        var result = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            Email = user.Email,
            Phone = user.Phone,
            Login = user.Login,
            Role = user.Role.Name,
            RoleId = user.RoleId,
            PostOfficeId = user.PostOfficeId,
            PostOfficeName = user.PostOffice != null ? user.PostOffice.Name : null,
            PassportSeries = user.PassportSeries,
            PassportNumber = user.PassportNumber,
            PassportIssuedBy = user.PassportIssuedBy,
            PassportIssueDate = user.PassportIssueDate,
            RegistrationAddress = user.RegistrationAddress,
            IsActive = user.IsActive
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEmployeeRequest request)
    {
        var exists = await _context.Users.AnyAsync(x =>
            x.Login.ToLower() == request.Login.ToLower() ||
            x.Email.ToLower() == request.Email.ToLower() ||
            x.Phone == request.Phone ||
            x.PassportNumber == request.PassportNumber);

        if (exists)
        {
            return BadRequest("Этот логин, email, телефон или паспорт уже используется");
        }

        var clientRole = await _context.Roles
            .FirstOrDefaultAsync(x => x.Name == "Client");

        if (clientRole == null)
            return BadRequest("Роль Client не найдена");

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Email = request.Email,
            Login = request.Login,
            MiddleName = request.MiddleName,
            PassportSeries = request.PassportSeries,
            PassportNumber = request.PassportNumber,
            PassportIssuedBy = request.PassportIssuedBy,
            PassportIssueDate = request.PassportIssueDate,
            RegistrationAddress = request.RegistrationAddress,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = clientRole.Id,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        if (!ModelState.IsValid)
        {
            return BadRequest("Проверьте правильность заполнения полей");
        }
        await _auditService.LogAsync(
            actionType: "Create",
            tableName: "Clients",
            recordId: user.Id.ToString(),
            newValues: AuditService.SerializeObject(new
            {
                request.FirstName,
                request.LastName,
                request.Login,
                request.Email,
                request.Phone
            })
        );

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateEmployeeRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id && x.Role.Name == "Client");

        if (user == null)
            return NotFound();
        var oldValues = AuditService.SerializeObject(new
        {
            user.FirstName,
            user.LastName,
            user.MiddleName,
            user.Phone,
            user.Email,
            user.Login,
            user.IsActive,
            user.PassportSeries,
            user.PassportNumber,
            user.PassportIssuedBy,
            user.PassportIssueDate,
            user.RegistrationAddress
        });

        var exists = await _context.Users.AnyAsync(x =>
            x.Id != id &&
            (x.Login.ToLower() == request.Login.ToLower() ||
             x.Email.ToLower() == request.Email.ToLower() ||
             x.Phone == request.Phone ||
             x.PassportNumber == request.PassportNumber));

        if (exists)
        {
            return BadRequest("Этот логин, email, телефон или паспорт уже используется");
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.MiddleName = request.MiddleName;
        user.Phone = request.Phone;
        user.Email = request.Email;
        user.Login = request.Login;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;
        user.PassportSeries = request.PassportSeries;
        user.PassportNumber = request.PassportNumber;
        user.PassportIssuedBy = request.PassportIssuedBy;
        user.PassportIssueDate = request.PassportIssueDate;
        user.RegistrationAddress = request.RegistrationAddress;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            if (request.Password.Length < 8)
            {
                return BadRequest("Пароль должен быть минимум 8 символов");
            }
            if (!request.Password.Any(char.IsLetter) || !request.Password.Any(char.IsDigit))
            {
                return BadRequest("Пароль должен содержать буквы и цифры");
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }

        await _context.SaveChangesAsync();

        if (!ModelState.IsValid)
        {
            return BadRequest("Проверьте правильность заполнения полей");
        }
        await _auditService.LogAsync(
            actionType: "Update",
            tableName: "Clients",
            recordId: id.ToString(),
            oldValues: oldValues,
            newValues: AuditService.SerializeObject(new
            {
                request.FirstName,
                request.LastName,
                request.MiddleName,
                request.Phone,
                request.Email,
                request.Login,
                request.IsActive,
                request.PassportSeries,
                request.PassportNumber,
                request.PassportIssuedBy,
                request.PassportIssueDate,
                request.RegistrationAddress
            })
        );

        return Ok();
    }
}