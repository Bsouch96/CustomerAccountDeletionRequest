using Microsoft.AspNetCore.Builder;
using CustomerAccountDeletionRequest.CustomExceptionMiddleware;

namespace CustomerAccountDeletionRequest.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
