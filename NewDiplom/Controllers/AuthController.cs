using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewDiplom.Data;
using NewDiplom.DTOs.Auth;
using NewDiplom.Entities;
using NewDiplom.Services;
using System.Runtime.ConstrainedExecution;

namespace NewDiplom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    private const int MAX_FAILED_ATTEMPTS = 10;
    private const int LOCKOUT_MINUTES = 5;

    public AuthController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(x => x.Login.ToLower() == request.Login.ToLower()))
        {
            ModelState.AddModelError("Login", "Логин уже занят");
        }

        if (await _context.Users.AnyAsync(x => x.Email.ToLower() == request.Email.ToLower()))
        {
            ModelState.AddModelError("Email", "Email уже используется");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var clientRole = await _context.Roles.FirstOrDefaultAsync(x => x.Name == "Client");

        var user = new User
        {
            Login = request.Login,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = clientRole!.Id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            IsDeleted = false,
            AccessFailedCount = 0,
            LockoutEnabled = true,
            LockoutEnd = null
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Login == request.Login && x.IsActive==true);

        if (user == null)
        {
            return Unauthorized(new { message = "Неверный логин или пароль" });
        }

        // ПРОВЕРКА АКТИВНОЙ БЛОКИРОВКИ 
        if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now)
        {
            var remainingMinutes = (int)Math.Ceiling((user.LockoutEnd.Value - DateTime.Now).TotalMinutes);
            return Unauthorized(new
            {
                message = $"Аккаунт заблокирован. Попробуйте через {remainingMinutes} минут.",
                remainingMinutes = remainingMinutes,
                isLocked = true
            });
        }

        // ПРОВЕРКА ПАРОЛЯ 
        var validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!validPassword || user.LockoutEnd > DateTime.Now)
        {
            if(user.LockoutEnd > DateTime.Now) {
                var remainingMinutes = (int)Math.Ceiling((user.LockoutEnd.Value - DateTime.Now).TotalMinutes);
                return Unauthorized(new
                {
                    message = $"Аккаунт заблокирован. Попробуйте через {remainingMinutes} минут.",
                    remainingMinutes = remainingMinutes,
                    isLocked = true
                });
            }
            user.AccessFailedCount++;
            await _context.SaveChangesAsync();

            if (user.AccessFailedCount >= MAX_FAILED_ATTEMPTS)
            {
                user.LockoutEnd = DateTime.Now.AddMinutes(LOCKOUT_MINUTES);
                await _context.SaveChangesAsync();

                return Unauthorized(new
                {
                    message = $"Аккаунт заблокирован на {LOCKOUT_MINUTES} минут",
                    remainingMinutes = LOCKOUT_MINUTES,
                    isLocked = true
                });
            }

            var remainingAttempts = MAX_FAILED_ATTEMPTS - user.AccessFailedCount;
            return Unauthorized(new
            {
                message = remainingAttempts > 0
                    ? $"Неверный логин или пароль. Осталось попыток: {remainingAttempts}"
                    : "Неверный логин или пароль",
                remainingAttempts = remainingAttempts
            });
        }

        // УСПЕШНЫЙ ВХОД
        user.AccessFailedCount = 0;
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user, user.Role!.Name);

        return Ok(new AuthResponse
        {
            Token = token,
            Login = user.Login,
            Role = user.Role.Name
        });
    }
}