using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Products;
using Services.Products.Create;
using Services.Products.Dto;
using Services.Products.Update;
using Services.Products.UpdateStock;

namespace Services.Products;
public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper)
    : IProductService
{
    public async Task<ServiceResult<List<ProductResponse>>> GetTopPriceProductsAsync(int count)
    {
        var products = await productRepository.GetTopPriceProductsAsync(count);
        var productsAsDto = mapper.Map<List<ProductResponse>>(products);

        return new ServiceResult<List<ProductResponse>>()
        {
            Data = productsAsDto
        };
    }

    public async Task<ServiceResult<List<ProductResponse>>> GetAllAsync()
    {
        var products = await productRepository.GetAll().ToListAsync();
        var productsAsDto = mapper.Map<List<ProductResponse>>(products);
        return ServiceResult<List<ProductResponse>>.Success(productsAsDto);
    }

    public async Task<ServiceResult<List<ProductResponse>>> GetPagedAllAsync(int pageNumber, int pageSize)
    {
        var products = await productRepository.GetAll()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var productsDto = mapper.Map<List<ProductResponse>>(products);
        return ServiceResult<List<ProductResponse>>.Success(productsDto);
    }

    public async Task<ServiceResult<ProductResponse>> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        var producAstDto = mapper.Map<ProductResponse>(product);
        return ServiceResult<ProductResponse>.Success(producAstDto);
    }

    public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
    {
        var isProductNameExist = await productRepository.Where(x => x.Name == request.Name).AnyAsync();

        if (isProductNameExist)
        {
            return ServiceResult<CreateProductResponse>.Fail("Ürün ismi veritabanında bulunmaktadır",
                HttpStatusCode.Conflict);
        }

        var product = mapper.Map<Product>(request);
        await productRepository.AddAsync(product);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id),
            $"/api/products/{product.Id}");
    }


    public async Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request)
    {
        var isProductNameExist =
            await productRepository.Where(x => x.Name == request.Name && x.Id != id).AnyAsync();

        if (isProductNameExist)
        {
            return ServiceResult.Fail("Ürün ismi veritabanında bulunmaktadır",
                HttpStatusCode.Conflict);
        }

        var product = mapper.Map<Product>(request);
        product.Id = id;
        productRepository.Update(product!);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> UpdateStockAsync(int id, UpdateProductStockRequest request)
    {
        var product = await productRepository.GetByIdAsync(id);
        product!.Stock = request.Stock;
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        productRepository.Delete(product!);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}