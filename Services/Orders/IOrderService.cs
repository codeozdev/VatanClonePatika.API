using Services.Orders.Dtos;

namespace Services.Orders;

public interface IOrderService
{
    Task<ServiceResult<OrderResponseDTO>> CreateOrderAsync(CreateOrderRequest request, string userId);
    Task<ServiceResult<List<OrderResponseDTO>>> GetUserOrdersAsync(string userId);
    Task<ServiceResult> DeleteOrderAsync(int orderId, string userId);
}