using Microsoft.AspNetCore.Identity;
using Repositories.Identity;

namespace Repositories.Seeds;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        try
        {
            // Admin rolünü oluştur
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole("Admin"));
                Console.WriteLine($"Admin rolü oluşturuldu: {roleResult.Succeeded}");
            }

            // Admin kullanıcısını oluştur
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, "Admin123!");
                Console.WriteLine($"Admin kullanıcısı oluşturuldu: {createResult.Succeeded}");

                if (createResult.Succeeded)
                {
                    var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine($"Admin rolü kullanıcıya eklendi: {roleResult.Succeeded}");

                    // Rolleri kontrol et
                    var roles = await userManager.GetRolesAsync(adminUser);
                    Console.WriteLine($"Kullanıcının rolleri: {string.Join(", ", roles)}");
                }
                else
                {
                    Console.WriteLine($"Kullanıcı oluşturma hataları: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                // Varolan kullanıcıya rol ata
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine($"Var olan kullanıcıya Admin rolü eklendi: {roleResult.Succeeded}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Seed işleminde hata: {ex.Message}");
            throw;
        }
    }
}
