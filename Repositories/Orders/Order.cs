using Repositories.Identity;

namespace Repositories.Orders;
public class Order : BaseEntity
{
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }

    // Navigation properties
    public string CustomerId { get; set; }
    public AppUser Customer { get; set; }

    // OrderProduct ilişkisi (çoka çok ilişki için)
    public List<OrderProduct> OrderProducts { get; set; } = null!;
}
