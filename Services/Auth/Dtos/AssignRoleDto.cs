namespace Services.Auth.Dtos;

public class AssignRoleDto
{
    public string Email { get; set; } = null!;
    public string RoleName { get; set; } = null!;
}