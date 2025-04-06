using Microsoft.AspNetCore.Mvc;
using Repositories.Products;
using Services.Filters;
using Services.Products;
using Services.Products.Create;
using Services.Products.Update;
using Services.Products.UpdateStock;

namespace VatanClonePatika.API.Controllers;

public class ProductsController(IProductService productService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        CreateActionResult(await productService.GetAllAsync());

    [HttpGet("{pageNumber:int}/{pageSize:int}")]
    public async Task<IActionResult> GetPagedAll([FromRoute] int pageNumber, int pageSize) =>
        CreateActionResult(await productService.GetPagedAllAsync(pageNumber, pageSize));

    [ServiceFilter(typeof(NotFoundFilter<Product>))]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get([FromRoute] int id) =>
        CreateActionResult(await productService.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request) =>
        CreateActionResult(await productService.CreateAsync(request));

    [ServiceFilter(typeof(NotFoundFilter<Product>))]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductRequest request) =>
        CreateActionResult(await productService.UpdateAsync(id, request));

    [ServiceFilter(typeof(NotFoundFilter<Product>))]
    [HttpPatch("stock/{id:int}")]
    public async Task<IActionResult> UpdateStock([FromRoute] int id, [FromBody] UpdateProductStockRequest request) =>
        CreateActionResult(await productService.UpdateStockAsync(id, request));

    [ServiceFilter(typeof(NotFoundFilter<Product>))]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id) =>
        CreateActionResult(await productService.DeleteAsync(id));
}
