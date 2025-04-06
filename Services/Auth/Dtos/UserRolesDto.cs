namespace Services.Auth.Dtos;

public class UserRolesDto
{
    public string Email { get; set; } = null!;
    public IList<string> Roles { get; set; } = new List<string>();
}