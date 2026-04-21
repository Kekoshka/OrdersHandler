using FluentValidation;
using System.Numerics;

namespace PaymentService.WebApi.Common.Extensions
{
    /// <summary>
    /// Методы расширения правил валидации
    /// </summary>
    public static class ValidationRulesExtensions
    {
        /// <summary>
        /// Правило валидации для чисел, числа должны быть >= 0
        /// </summary>
        /// <typeparam name="T">Обобщенный тип T</typeparam>
        /// <typeparam name="TNumber">Обобщенный числовой тип</typeparam>
        /// <param name="ruleBuilder">Правило валидации</param>
        /// <returns>Правило валидации</returns>
        public static IRuleBuilderOptions<T, TNumber> ValidatePositiveNumber<T, TNumber>(
        this IRuleBuilder<T, TNumber> ruleBuilder)
        where TNumber : INumber<TNumber>
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(TNumber.Zero)
                .WithMessage("Number must be a positive number or equal to 0");
        }
        /// <summary>
        /// Правило валидации для номеров телефона 
        /// </summary>
        /// <typeparam name="T">обобщенный тип Т</typeparam>
        /// <param name="ruleBuilder">Правило валидации</param>
        /// <returns>Правило валидации</returns>
        public static IRuleBuilderOptions<T,string> ValidatePhoneNumber<T>(this IRuleBuilder<T,string> ruleBuilder)
        {
            return ruleBuilder
                .Matches("^((8|\\+7)[\\- ]?)?(\\(?\\d{3}\\)?[\\- ]?)?[\\d\\- ]{7,10}$").WithMessage("Enter phone number, like +71231234567");
        }
    }
}
