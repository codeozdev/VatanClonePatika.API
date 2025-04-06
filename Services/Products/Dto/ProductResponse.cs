namespace Services.Products.Dto;
public record ProductResponse(int Id, string Name, decimal Price, int Stock, int CategoryId);