namespace Services.Orders.Dtos;

public class OrderResponseDTO
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemResponseDTO> Items { get; set; } = [];
}

public class OrderItemResponseDTO
{
    public string ProductName { get; set; }
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}
