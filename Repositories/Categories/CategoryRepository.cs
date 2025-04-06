using Microsoft.EntityFrameworkCore;
namespace Repositories.Categories;

public class CategoryRepository(AppDbContext context) : GenericRepository<Category>(context), ICategoryRepository
{
    public Task<Category?> GetCategoryWithProductsAsync(int id) => Context.Categories
        .Include(x => x.Products)
        .FirstOrDefaultAsync(x => x.Id == id);

    public IQueryable<Category> GetCategoryWithProducts() => Context.Categories.Include(x => x.Products).AsQueryable();
}
