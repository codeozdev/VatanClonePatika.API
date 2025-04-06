using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Identity;
public class AuthRepository(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    RoleManager<IdentityRole> roleManager) : IAuthRepository
{
    public async Task<IdentityResult> RegisterAsync(AppUser user, string password)
    {
        user.UserName = user.Email;
        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // Eğer hiç kullanıcı yoksa ilk kullanıcıyı admin yap
            if (!userManager.Users.Any())
            {
                // Admin rolü yoksa oluştur
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                await userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                // Diğer kullanıcıları Customer yap
                if (!await roleManager.RoleExistsAsync("Customer"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Customer"));
                }
                await userManager.AddToRoleAsync(user, "Customer");
            }
        }

        return result;
    }

    public async Task<SignInResult> LoginAsync(string email, string password, bool isPersistent)
    {
        // Email ile giriş yapılacak
        return await signInManager.PasswordSignInAsync(email, password, isPersistent, false);
    }

    public async Task LogoutAsync()
    {
        await signInManager.SignOutAsync();
    }

    // Rol işlemleri
    public async Task<IdentityResult> CreateRoleAsync(string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return IdentityResult.Failed(new IdentityError { Description = $"{roleName} rolü zaten mevcut." });
        }
        return await roleManager.CreateAsync(new IdentityRole(roleName));
    }

    public async Task<IdentityResult> DeleteRoleAsync(string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = $"{roleName} rolü bulunamadı." });
        }
        return await roleManager.DeleteAsync(role);
    }

    public async Task<IList<string>> GetRolesAsync()
    {
        return await roleManager.Roles
            .Select(r => r.Name!)
            .Where(name => name != null)
            .ToListAsync();
    }

    // Kullanıcı-Rol işlemleri
    public async Task<IdentityResult> AssignRoleToUserAsync(string email, string roleName)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = $"{email} kullanıcısı bulunamadı." });
        }

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            return IdentityResult.Failed(new IdentityError { Description = $"{roleName} rolü bulunamadı." });
        }

        return await userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<IdentityResult> RemoveRoleFromUserAsync(string email, string roleName)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = $"{email} kullanıcısı bulunamadı." });
        }

        return await userManager.RemoveFromRoleAsync(user, roleName);
    }

    public async Task<IList<(AppUser User, IList<string> Roles)>> GetAllUsersWithRolesAsync()
    {
        var users = await userManager.Users.ToListAsync();
        var result = new List<(AppUser User, IList<string> Roles)>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            result.Add((user, roles));
        }

        return result;
    }

    public async Task<AppUser> GetUserByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email)
            ?? throw new InvalidOperationException($"Kullanıcı bulunamadı: {email}");
    }

    public async Task<IList<string>> GetUserRolesAsync(AppUser user)
    {
        return await userManager.GetRolesAsync(user);
    }
}
