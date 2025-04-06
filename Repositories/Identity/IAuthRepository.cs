using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Repositories.Identity;
public interface IAuthRepository
{
    Task<IdentityResult> RegisterAsync(AppUser user, string password);
    Task<SignInResult> LoginAsync(string email, string password, bool isPersistent);
    Task LogoutAsync();
    Task<AppUser> GetUserByEmailAsync(string email);
    Task<IList<string>> GetUserRolesAsync(AppUser user);

    // Rol işlemleri
    Task<IdentityResult> CreateRoleAsync(string roleName);
    Task<IdentityResult> DeleteRoleAsync(string roleName);
    Task<IList<string>> GetRolesAsync();


    // Kullanıcı-Rol işlemleri
    Task<IdentityResult> AssignRoleToUserAsync(string email, string roleName);
    Task<IList<(AppUser User, IList<string> Roles)>> GetAllUsersWithRolesAsync();
}