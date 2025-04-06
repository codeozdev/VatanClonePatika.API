using FluentValidation;

namespace Services.Categories.Update;
public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    // constructor method
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategori ismi gereklidir")
            .Length(3, 50).WithMessage("Kategori ismi 3 ile 50 karakter arasında olmalıdır");
    }
}
