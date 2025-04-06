using Microsoft.EntityFrameworkCore;

namespace Repositories.Products;
public class ProductRepository(AppDbContext context) : GenericRepository<Product>(context), IProductRepository
{
    public Task<List<Product>> GetTopPriceProductsAsync(int count) => Context.Products.OrderByDescending(x => x.Price).Take(count).ToListAsync();
}
