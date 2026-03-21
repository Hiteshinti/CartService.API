using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

 namespace CartService.Core.MiddleWare
{
    public class ExceptionMiddleWare
    {

        private readonly RequestDelegate _context;
        private readonly ILogger<ExceptionMiddleWare> _logger;

        public ExceptionMiddleWare(RequestDelegate context, ILogger<ExceptionMiddleWare> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _context(httpContext);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception while processing request.");
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    Message = "An unexpected error occurred."
                });
            }

        }

    }

}
