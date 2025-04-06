namespace Repositories.Orders;
public interface IOrderRepository : IGenericRepository<Order>
{
    Task<List<Order>> GetUserOrdersWithDetailsAsync(string userId);
    Task<bool> DeleteOrderWithProductsAsync(int orderId);
}