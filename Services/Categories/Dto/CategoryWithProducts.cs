using Services.Products.Dto;

namespace Services.Categories.Dto;
public record CategoryWithProductsDto(int Id, string Name, List<ProductResponse> Products);