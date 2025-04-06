using Microsoft.AspNetCore.Identity;
using Repositories.Orders;

namespace Repositories.Identity;
public class AppUser : IdentityUser
{
    public List<Order> Orders { get; set; }
}
