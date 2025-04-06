using Microsoft.AspNetCore.Mvc;
using Repositories.Categories;
using Services.Categories;
using Services.Categories.Create;
using Services.Categories.Update;
using Services.Filters;

namespace VatanClonePatika.API.Controllers;

public class CategoriesController(ICategoryService categoryService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        CreateActionResult(await categoryService.GetAllAsync());

    [ServiceFilter(typeof(NotFoundFilter<Category>))]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => CreateActionResult(await categoryService.GetByIdAsync(id));

    [HttpGet("{id:int}/products")] // custom methodumuz birtane veri alan
    public async Task<IActionResult> GetCategoryWithProducts(int id) =>
        CreateActionResult(await categoryService.GetCategoryWithProductsAsync(id));

    [HttpGet("products")] // custom methodumuz birden fazla veri alan
    public async Task<IActionResult> GetCategoryWithProducts() =>
        CreateActionResult(await categoryService.GetCategoryWithProductsAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request) =>
        CreateActionResult(await categoryService.CreateAsync(request));

    [ServiceFilter(typeof(NotFoundFilter<Category>))]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryRequest request) =>
        CreateActionResult(await categoryService.UpdateAsync(id, request));

    [ServiceFilter(typeof(NotFoundFilter<Category>))]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id) =>
        CreateActionResult(await categoryService.DeleteAsync(id));
}