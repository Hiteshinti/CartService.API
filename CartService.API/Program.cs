using CartService.Core;
using CartService.Core.IProviders;
using CartService.Core.Mappers;
using CartService.Core.MiddleWare;
using CartService.Core.Providers;
using CartService.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Identity.Client.RP;
using Polly.Extensions.Http;
using Polly;
using System.Net;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Logging.AddConsole();   



builder.Services.AddInfraStructure(builder.Configuration);
builder.Services.AddCore(builder.Configuration);
builder.Services.AddScoped<IUserProvider,UserProvider>();
builder.Services.AddAutoMapper(cfg => cfg.LicenseKey = "<License Key Here>", typeof(CartItemsMapping).Assembly);
builder.Services.AddSwaggerGen();


// Add httpclient for internal service communcation 

builder.Services.AddHttpClient("MyApiClient").AddPolicyHandler(GetRetryPolicy());
builder.Services.AddHttpContextAccessor();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = $"{builder.Configuration["REDIS_HOST"]}:{builder.Configuration["REDIS_PORT"]}";

});
var connectionString =
    $"{builder.Configuration["REDIS_HOST"]}:{builder.Configuration["REDIS_PORT"]}";

Console.WriteLine($"Redis Connection String = {connectionString}");

//app.MapGet("/", () => "Hello World!");
//builder.WebHost.UseUrls("http://*:9090");
var app = builder.Build();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ExceptionMiddleWare>();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.Run();
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // 5xx, 408, network failures
        .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(
            3,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryCount, context) =>
            {
                Console.WriteLine(
                    $"Retry {retryCount} after {timespan.TotalSeconds}s");
            });
}
