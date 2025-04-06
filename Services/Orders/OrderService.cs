using System.Net;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Orders;
using Repositories.Products;
using Services.Orders.Dtos;

namespace Services.Orders;

public class OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IUnitOfWork unitOfWork) : IOrderService
{


    public async Task<ServiceResult<OrderResponseDTO>> CreateOrderAsync(CreateOrderRequest request, string userId)
    {
        var order = new Order
        {
            CustomerId = userId,
            OrderDate = DateTime.UtcNow,
            OrderProducts = []
        };

        decimal totalAmount = 0;
        foreach (var item in request.Items)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                return ServiceResult<OrderResponseDTO>.Fail($"Ürün bulunamadı: {item.ProductId}", HttpStatusCode.NotFound);

            order.OrderProducts.Add(new OrderProduct
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            });

            totalAmount += product.Price * item.Quantity;
        }

        order.TotalAmount = totalAmount;
        await orderRepository.AddAsync(order);
        await unitOfWork.SaveChangesAsync();

        var response = new OrderResponseDTO
        {
            OrderId = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Items = [.. order.OrderProducts.Select(item =>
            {
                var product = productRepository.GetByIdAsync(item.ProductId).Result;
                return new OrderItemResponseDTO
                {
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                    Quantity = item.Quantity,
                    TotalPrice = product.Price * item.Quantity
                };
            })]
        };

        return ServiceResult<OrderResponseDTO>.SuccessAsCreated(response, $"/api/orders/{order.Id}");
    }

    public async Task<ServiceResult<List<OrderResponseDTO>>> GetUserOrdersAsync(string userId)
    {
        var orders = await orderRepository.GetUserOrdersWithDetailsAsync(userId);

        var orderDtos = orders.Select(order => new OrderResponseDTO
        {
            OrderId = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Items = [.. order.OrderProducts.Select(item => new OrderItemResponseDTO
            {
                ProductName = item.Product.Name,
                ProductPrice = item.Product.Price,
                Quantity = item.Quantity,
                TotalPrice = item.Product.Price * item.Quantity
            })]
        }).ToList();

        return ServiceResult<List<OrderResponseDTO>>.Success(orderDtos);
    }

    public async Task<ServiceResult> DeleteOrderAsync(int orderId, string userId)
    {
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null)
            return ServiceResult.Fail($"{orderId} numaralı sipariş bulunamadı.", HttpStatusCode.NotFound);

        var result = await orderRepository.DeleteOrderWithProductsAsync(orderId);
        if (!result)
            return ServiceResult.Fail($"{orderId} numaralı sipariş silinemedi.", HttpStatusCode.BadRequest);

        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}