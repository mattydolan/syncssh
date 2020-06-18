using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace emri_service.Middleware
{
	public class EnableRequestBufferingMiddleware
    {
        private readonly RequestDelegate _next;

        public EnableRequestBufferingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();
            await _next(context);
        }
    }

    public static class EnableRequestBufferingExtension
    {
        public static IApplicationBuilder UseEnableRequestBuffering(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EnableRequestBufferingMiddleware>();
        }
    }
}
