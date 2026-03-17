using PaymentService.WebApi.Middlewares;

namespace PaymentService.WebApi.Common.Extensions
{
    public static class MiddlewaresExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
