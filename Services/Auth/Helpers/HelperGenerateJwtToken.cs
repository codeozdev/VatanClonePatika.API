using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.Auth.Helpers;

public class HelperGenerateJwtToken
{
    public static string GenerateToken(string userId, string email, string key, string issuer, string audience, IList<string>? roles = null)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, userId),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.NameIdentifier, userId)
    };

        // Kullanıcı rollerini claim'e ekliyoruz
        if (roles != null)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30), // Token geçerlilik süresi
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}