namespace Services.Orders.Dtos;

public class CreateOrderRequest
{
    public List<OrderItemDTO> Items { get; set; } = [];
}

public class OrderItemDTO
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}