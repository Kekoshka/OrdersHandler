using ProjectManagementSystemBackend.Middlewares;

namespace OrderService.WebApi.Extensions
{
    public static class MiddlewaresExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
