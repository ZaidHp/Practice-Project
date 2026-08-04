using Microsoft.EntityFrameworkCore;
using PharmaTrack.Domain.Entities;

namespace PharmaTrack.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        if (!await context.Roles.AnyAsync())
        {
            var adminRole = new Role { RoleName = "Admin", Description = "Full system access" };
            var managerRole = new Role { RoleName = "Store Manager", Description = "Manages stock & purchases" };
            var pharmacistRole = new Role { RoleName = "Pharmacist", Description = "Handles sales & stock checks" };

            await context.Roles.AddRangeAsync(adminRole, managerRole, pharmacistRole);
            await context.SaveChangesAsync();

            var adminUser = new User
            {
                RoleId = adminRole.RoleId,
                Username = "admin",
                Email = "admin@pharmatrack.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FullName = "System Administrator"
            };

            await context.Users.AddAsync(adminUser);
        }

        await context.SaveChangesAsync();
    }
}
