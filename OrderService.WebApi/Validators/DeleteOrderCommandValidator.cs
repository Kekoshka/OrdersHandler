using FluentValidation;
using OrderService.WebApi.Extensions;
using OrderService.WebApi.Mediatr.Commands;

namespace OrderService.WebApi.Validators
{
    /// <summary>
    /// Валидатор команд удаления заказов
    /// </summary>
    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
    {
        /// <summary>
        /// Валидирует команды удаления заказов
        /// </summary>
        public DeleteOrderCommandValidator() 
        {
            RuleFor(x => x.OrderId).ValidatePositiveNumber();
        }
    }
}
