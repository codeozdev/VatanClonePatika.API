using AutoMapper;
using Repositories.Products;
using Services.Products.Create;
using Services.Products.Dto;
using Services.Products.Update;

namespace Services.Products;
public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductResponse>().ReverseMap();
        CreateMap<CreateProductRequest, Product>().ForMember(dest => dest.Name,
            opt => opt.MapFrom(src => src.Name.ToLowerInvariant()));

        CreateMap<UpdateProductRequest, Product>().ForMember(dest => dest.Name,
            opt => opt.MapFrom(src => src.Name.ToLowerInvariant()));
    }
}