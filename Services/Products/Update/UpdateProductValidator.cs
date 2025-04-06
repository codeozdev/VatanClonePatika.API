using FluentValidation;

namespace Services.Products.Update;
public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün ismi gereklidir")
            .Length(3, 50).WithMessage("Ürün ismi 3 ile 50 karakter arasında olmalıdır");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır");

        RuleFor(x => x.Stock)
            .InclusiveBetween(1, 100).WithMessage("Stok miktari 1 ile 100 arasında olmalıdır");
        RuleFor(x => x.CategoryId)
           .GreaterThan(0).WithMessage("Ürün kategory 0'dan büyük olmalıdır");
    }
}