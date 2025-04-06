using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Categories;
using Services.Categories.Create;
using Services.Categories.Dto;
using Services.Categories.Update;

namespace Services.Categories;
public class CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper)
    : ICategoryService
{

    public async Task<ServiceResult<CategoryWithProductsDto>> GetCategoryWithProductsAsync(int categoryId)
    {
        var category = await categoryRepository.GetCategoryWithProductsAsync(categoryId);

        if (category is null)
        {
            return ServiceResult<CategoryWithProductsDto>.Fail($"id {categoryId} ile kategori bulunamadı");
        }

        var categoryAsDto = mapper.Map<CategoryWithProductsDto>(category);
        return ServiceResult<CategoryWithProductsDto>.Success(categoryAsDto);
    }

    public async Task<ServiceResult<List<CategoryWithProductsDto>>> GetCategoryWithProductsAsync()
    {
        var category = await categoryRepository.GetCategoryWithProducts().ToListAsync();
        var categoryAsDto = mapper.Map<List<CategoryWithProductsDto>>(category);
        return ServiceResult<List<CategoryWithProductsDto>>.Success(categoryAsDto);
    }


    public async Task<ServiceResult<List<CategoryResponse>>> GetAllAsync()
    {
        var categories = await categoryRepository.GetAll().ToListAsync();
        var categoriesAsDto = mapper.Map<List<CategoryResponse>>(categories);
        return ServiceResult<List<CategoryResponse>>.Success(categoriesAsDto);
    }

    public async Task<ServiceResult<CategoryResponse>> GetByIdAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        var categoryAsDto = mapper.Map<CategoryResponse>(category);
        return ServiceResult<CategoryResponse>.Success(categoryAsDto);
    }

    public async Task<ServiceResult<CreateCategoryResponse>> CreateAsync(CreateCategoryRequest request)
    {
        var isCategoryNameExist = await categoryRepository.Where(x => x.Name == request.Name).AnyAsync();

        if (isCategoryNameExist)
        {
            return ServiceResult<CreateCategoryResponse>.Fail("Category ismi veritabanında bulunmaktadır",
                HttpStatusCode.Conflict);
        }

        var category = mapper.Map<Category>(request);
        await categoryRepository.AddAsync(category);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult<CreateCategoryResponse>.SuccessAsCreated(new CreateCategoryResponse(category.Id),
            $"/api/categories/{category.Id}");
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var isProductNameExist =
            await categoryRepository.Where(x => x.Name == request.Name && x.Id != id).AnyAsync();

        if (isProductNameExist)
        {
            return ServiceResult.Fail("Category ismi veritabanında bulunmaktadır",
                HttpStatusCode.Conflict);
        }

        var category = mapper.Map<Category>(request);
        category.Id = id;
        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        categoryRepository.Delete(category!);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
