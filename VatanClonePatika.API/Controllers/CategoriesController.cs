using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Categories;
using Services.Categories;
using Services.Categories.Create;
using Services.Categories.Update;
using Services.Filters;

namespace VatanClonePatika.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CategoriesController(ICategoryService categoryService) : CustomBaseController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        CreateActionResult(await categoryService.GetAllAsync());

    [AllowAnonymous]
    [ServiceFilter(typeof(NotFoundFilter<Category>))]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => CreateActionResult(await categoryService.GetByIdAsync(id));

    [AllowAnonymous]
    [HttpGet("{id:int}/products")]
    public async Task<IActionResult> GetCategoryWithProducts(int id) =>
        CreateActionResult(await categoryService.GetCategoryWithProductsAsync(id));

    [AllowAnonymous]
    [HttpGet("products")]
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