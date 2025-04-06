using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Auth;
using Services.Auth.Dtos;
using Services.Auth.DTOs;
using System.Security.Claims;

namespace VatanClonePatika.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{


    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage) });
        }

        var (result, token) = await authService.RegisterAsync(model);

        if (result.Succeeded)
        {
            return Ok(new
            {
                message = "Başarıyla kayıt oldunuz",
                token
            });
        }

        return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO model)
    {
        // ModelState kontrolü
        if (!ModelState.IsValid)
        {
            return BadRequest(new { errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage) });
        }

        // Login işlemi
        var (result, token) = await authService.LoginAsync(model);

        if (result.Succeeded)
        {
            // Başarılı login sonrası token'ı döndürüyoruz
            return Ok(new
            {
                message = "Başarıyla giriş yaptınız",
                token
            });
        }

        // Hatalı giriş
        return Unauthorized(new { message = "Geçersiz email veya şifre" });
    }



    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync();
        return Ok(new { message = "Başarıyla çıkış yaptınız" });
    }

    // Rol yönetimi endpoint'leri
    [Authorize(Roles = "Admin")]
    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] RoleDto model)
    {
        var result = await authService.CreateRoleAsync(model);
        if (result.Succeeded)
        {
            return Ok($"{model.Name} rolü başarıyla oluşturuldu.");
        }
        return BadRequest(result.Errors);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("roles/{roleName}")]
    public async Task<IActionResult> DeleteRole([FromRoute] string roleName)
    {
        var result = await authService.DeleteRoleAsync(roleName);
        if (result.Succeeded)
        {
            return Ok($"{roleName} rolü başarıyla silindi.");
        }
        return BadRequest(result.Errors);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        if (!User.IsInRole("Admin"))
        {
            return StatusCode(403, new { message = "Bu işlem için yetkiniz bulunmamaktadır." });
        }

        var roles = await authService.GetRolesAsync();
        return Ok(roles);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("users/roles")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
    {
        if (!User.IsInRole("Admin"))
        {
            return StatusCode(403, new { message = "Bu işlem için yetkiniz bulunmamaktadır." });
        }

        var result = await authService.AssignRoleToUserAsync(new UserRoleDto
        {
            Email = model.Email,
            RoleName = model.RoleName
        });

        if (result.Succeeded)
        {
            return Ok($"{model.Email} kullanıcısına {model.RoleName} rolü başarıyla atandı.");
        }
        return BadRequest(result.Errors);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("users-with-roles")]
    public async Task<IActionResult> GetAllUsersWithRoles()
    {
        var usersWithRoles = await authService.GetAllUsersWithRolesAsync();
        return Ok(usersWithRoles);
    }
}