
using Microsoft.EntityFrameworkCore;

namespace Repositories.Orders;
internal class OrderRepository(AppDbContext context) : GenericRepository<Order>(context), IOrderRepository
{
    public async Task<List<Order>> GetUserOrdersWithDetailsAsync(string userId)
    {
        return await Context.Orders
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .Where(o => o.CustomerId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<bool> DeleteOrderWithProductsAsync(int orderId)
    {
        var order = await Context.Orders.FindAsync(orderId);
        if (order == null)
            return false;

        var orderProducts = await Context.OrderProducts
            .Where(op => op.OrderId == orderId)
            .ToListAsync();

        Context.OrderProducts.RemoveRange(orderProducts);
        Context.Orders.Remove(order);

        return true;
    }
}


// public class ProductRepository(AppDbContext context) : GenericRepository<Product>(context), IProductRepository