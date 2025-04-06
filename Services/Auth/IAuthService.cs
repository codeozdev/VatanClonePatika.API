using Microsoft.AspNetCore.Identity;
using Services.Auth.Dtos;
using Services.Auth.DTOs;

namespace Services.Auth;
public interface IAuthService
{
    Task<(IdentityResult Result, string? Token)> RegisterAsync(RegisterDto model);
    Task<(SignInResult Result, string? Token)> LoginAsync(LoginDTO model);
    Task LogoutAsync();

    // Rol işlemleri
    Task<IdentityResult> CreateRoleAsync(RoleDto model);
    Task<IdentityResult> DeleteRoleAsync(string roleName);
    Task<IList<RoleDto>> GetRolesAsync();

    // Kullanıcı-Rol işlemleri
    Task<IdentityResult> AssignRoleToUserAsync(UserRoleDto model);
    Task<IList<UserRolesDto>> GetAllUsersWithRolesAsync();
}