using FluentValidation;

namespace Services.Orders.Dtos;
public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Sipariş içeriği boş olamaz.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir ürün seçmelisiniz.");

            item.RuleFor(x => x.Quantity)
                .InclusiveBetween(1, 10)
                .WithMessage("Ürün miktarı 1 ile 10 arasında olmalıdır.");
        });
    }
}