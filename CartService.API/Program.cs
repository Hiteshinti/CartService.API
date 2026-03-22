using CartService.Core;
using CartService.Core.IProviders;
using CartService.Core.Mappers;
using CartService.Core.MiddleWare;
using CartService.Core.Providers;
using CartService.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Identity.Client.RP;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Logging.AddConsole();   

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.WithOrigins("https://localhost:5243")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
          
        });
});

builder.Services.AddInfraStructure();
builder.Services.AddCore();
builder.Services.AddScoped<IUserProvider,UserProvider>();
builder.Services.AddAutoMapper(cfg => cfg.LicenseKey = "<License Key Here>", typeof(CartItemsMapping).Assembly);
builder.Services.AddSwaggerGen();


// Add httpclient for internal service communcation 

builder.Services.AddHttpClient("MyApiClient");
builder.Services.AddHttpContextAccessor();


//app.MapGet("/", () => "Hello World!");
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
