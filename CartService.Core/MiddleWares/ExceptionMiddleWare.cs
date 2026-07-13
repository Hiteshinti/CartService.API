using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

 namespace CartService.Core.MiddleWare
{
    public class ExceptionMiddleWare
    {

        private readonly RequestDelegate _context;
        private readonly ILogger<ExceptionMiddleWare> _logger;
        private readonly IConfiguration _configuration;
        public ExceptionMiddleWare(RequestDelegate context, ILogger<ExceptionMiddleWare> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration; 
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _context(httpContext);

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.GetType().ToString()}:{ex.Message}");
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                _logger.LogError($"Exception: {ex.Message}");
                _logger.LogError($"HOST:{_configuration["RabitMQ_HOST"]+_configuration["RabitMQ_PORT"]+_configuration["RabitMQ_USER"]+ 
                    _configuration["RabitMQ_PASSWORD"]}");
               
                await httpContext.Response.WriteAsync(ex.Message );
            }

        }

    }

}
