using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Repositories.Identity;
using Services.Auth.Dtos;
using Services.Auth.DTOs;
using Services.Auth.Helpers;

namespace Services.Auth;

public class AuthService(IAuthRepository authRepository, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : IAuthService
{

    private bool IsUserInRole(string roleName)
    {
        return httpContextAccessor.HttpContext?.User?.IsInRole(roleName) ?? false;
    }

    private IdentityResult CreateForbiddenError()
    {
        return IdentityResult.Failed(new IdentityError
        {
            Description = "Bu işlem için yetkiniz bulunmamaktadır."
        });
    }

    public async Task<(IdentityResult Result, string? Token)> RegisterAsync(RegisterDto model)
    {
        // Şifrelerin eşleşip eşleşmediğini kontrol ediyoruz
        if (model.Password != model.PasswordConfirm)
        {
            return (IdentityResult.Failed(new IdentityError
            {
                Description = "Şifreler eşleşmiyor"
            }), null);
        }

        // Modeli AppUser'a dönüştürüyoruz
        var user = mapper.Map<AppUser>(model);

        // Kullanıcıyı kaydediyoruz
        var result = await authRepository.RegisterAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Eğer kullanıcı başarılı bir şekilde kaydedildiyse, token oluşturuyoruz
            var token = HelperGenerateJwtToken.GenerateToken(
                user.Id,                  // 👈 UserId'yi token'a ekliyoruz
                user.Email!,
                configuration["Jwt:Key"]!,
                configuration["Jwt:Issuer"]!,
                configuration["Jwt:Audience"]!
            );

            return (result, token);
        }

        // Başarısız ise null döndürüyoruz
        return (result, null);
    }


    public async Task<(SignInResult, string)> LoginAsync(LoginDTO model)
    {
        var user = await authRepository.GetUserByEmailAsync(model.Email);
        if (user == null)
            return (SignInResult.Failed, null);

        var result = await authRepository.LoginAsync(model.Email, model.Password, model.RememberMe);
        if (!result.Succeeded)
            return (result, null);

        // Kullanıcının rollerini al
        var roles = await authRepository.GetUserRolesAsync(user); // Var olan metodu kullanıyoruz

        var token = HelperGenerateJwtToken.GenerateToken(
            user.Id,
            user.Email,
            configuration["Jwt:Key"]!,
            configuration["Jwt:Issuer"]!,
            configuration["Jwt:Audience"]!,
            roles  // Rolleri token'a ekliyoruz
        );

        return (result, token);
    }



    public async Task LogoutAsync()
    {
        await authRepository.LogoutAsync();
    }

    // Rol işlemleri
    public async Task<IdentityResult> CreateRoleAsync(RoleDto model)
    {
        if (!IsUserInRole("Admin"))
        {
            return CreateForbiddenError();
        }
        return await authRepository.CreateRoleAsync(model.Name);
    }

    public async Task<IdentityResult> DeleteRoleAsync(string roleName)
    {
        if (!IsUserInRole("Admin"))
        {
            return CreateForbiddenError();
        }
        return await authRepository.DeleteRoleAsync(roleName);
    }

    public async Task<IList<RoleDto>> GetRolesAsync()
    {
        if (!IsUserInRole("Admin"))
        {
            return [];
        }
        var roles = await authRepository.GetRolesAsync();
        return [.. roles.Select(r => new RoleDto { Name = r })];
    }

    // Kullanıcı-Rol işlemleri
    public async Task<IdentityResult> AssignRoleToUserAsync(UserRoleDto model)
    {
        if (!IsUserInRole("Admin"))
        {
            return CreateForbiddenError();
        }
        return await authRepository.AssignRoleToUserAsync(model.Email, model.RoleName);
    }


    public async Task<IList<UserRolesDto>> GetAllUsersWithRolesAsync()
    {
        if (!IsUserInRole("Admin"))
        {
            return [];
        }
        var usersWithRoles = await authRepository.GetAllUsersWithRolesAsync();
        return [.. usersWithRoles.Select(x => new UserRolesDto
        {
            Email = x.User.Email!,
            Roles = x.Roles
        })];
    }
}

