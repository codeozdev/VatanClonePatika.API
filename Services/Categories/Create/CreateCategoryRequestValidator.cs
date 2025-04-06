using FluentValidation;

namespace Services.Categories.Create;
public class UpdateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    // constructor method
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategori ismi gereklidir")
            .Length(3, 50).WithMessage("Kategori ismi 3 ile 50 karakter arasında olmalıdır");
    }
}
