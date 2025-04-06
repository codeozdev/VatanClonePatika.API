using Repositories.Categories;
using Repositories.Orders;

namespace Repositories.Products
{
    public class Product: BaseEntity
    {
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // Foreign key
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // OrderProduct ilişkisi (çoka çok ilişki için)
        public List<OrderProduct> OrderProducts { get; set; } = null!;
    }
}
