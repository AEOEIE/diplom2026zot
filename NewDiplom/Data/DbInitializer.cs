using Microsoft.EntityFrameworkCore;
using NewDiplom.Entities;

namespace NewDiplom.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new Role
                {
                    Name = "Admin",
                    Description = "Администратор"
                },

                new Role
                {
                    Name = "Operator",
                    Description = "Оператор"
                },

                new Role
                {
                    Name = "ServiceManager",
                    Description = "Сервис менеджер"
                },

                new Role
                {
                    Name = "DepartmentHead",
                    Description = "Руководитель отделения"
                },

                new Role
                {
                    Name = "Client",
                    Description = "Клиент"
                }
            };

            context.Roles.AddRange(roles);

            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync())
        {
            var adminRole = await context.Roles
                .FirstAsync(x => x.Name == "Admin");

            var admin = new User
            {
                FirstName = "System",
                LastName = "Admin",

                Phone = "0000000000",

                Login = "admin",

                Email = "admin@newdiplom.local",

                PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678"),

                RoleId = adminRole.Id,

                IsActive = true,

                IsDeleted = false,

                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(admin);

            await context.SaveChangesAsync();
        }
        if (!await context.ShipmentStatuses.AnyAsync())
        {
            var statuses = new List<ShipmentStatus>
            {
                new ShipmentStatus
                {
                    Name = "Принято",
                    IsFinal = false
                },

                new ShipmentStatus
                {
                    Name = "В пути",
                    IsFinal = false
                },

                new ShipmentStatus
                {
                    Name = "Прибыло в отделение",
                    IsFinal = false
                },

                new ShipmentStatus
                {
                    Name = "Выдано",
                    IsFinal = true
                },

                new ShipmentStatus
                {
                    Name = "Отменено",
                    IsFinal = true
                }
            };

            context.ShipmentStatuses.AddRange(statuses);

            await context.SaveChangesAsync();
        }
    }
}