using FluentValidation;
using OrderService.WebApi.Common.Extensions;
using OrderService.WebApi.Mediatr;

namespace OrderService.WebApi.Common.Validators
{
    /// <summary>
    /// Валидатор команд создания заказов
    /// </summary>
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        /// <summary>
        /// Валидирует команды создания заказов
        /// </summary>
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.Price).ValidatePositiveNumber();
            RuleFor(x => x.PhoneNumber).ValidatePhoneNumber();
            RuleFor(x => x.EmailClient).EmailAddress();
            RuleFor(x => x.ProductId).ValidatePositiveNumber();
            RuleFor(x => x.Amount).ValidatePositiveNumber();
        }
    }
}
