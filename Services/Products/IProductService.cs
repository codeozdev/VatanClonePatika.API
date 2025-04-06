using Services.Products.Create;
using Services.Products.Dto;
using Services.Products.Update;
using Services.Products.UpdateStock;

namespace Services.Products;
public interface IProductService
{
    Task<ServiceResult<List<ProductResponse>>> GetTopPriceProductsAsync(int count);
    Task<ServiceResult<List<ProductResponse>>> GetAllAsync();
    Task<ServiceResult<List<ProductResponse>>> GetPagedAllAsync(int pageNumber, int pageSize);
    Task<ServiceResult<ProductResponse>> GetByIdAsync(int id);
    Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request);
    Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request);
    Task<ServiceResult> UpdateStockAsync(int id, UpdateProductStockRequest request);
    Task<ServiceResult> DeleteAsync(int id);
}
