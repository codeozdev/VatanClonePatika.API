using Repositories.Products;

namespace Repositories.Orders;
public class OrderProduct
{
    // Order iliskisi
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    // Product iliskisi
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // Sipariş edilen miktar
    public int Quantity { get; set; }
}
