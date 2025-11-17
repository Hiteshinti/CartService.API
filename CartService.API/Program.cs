using CartService.Core;
using CartService.Core.IProviders;
using CartService.Core.Mappers;
using CartService.Core.MiddleWare;
using CartService.Core.Providers;
using CartService.Infrastructure;
using Microsoft.Identity.Client.RP;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
          
        });
});

builder.Services.AddInfraStructure();
builder.Services.AddCore();
builder.Services.AddScoped<IUserProvider,UserProvider>();
builder.Services.AddAutoMapper(cfg => cfg.LicenseKey = "<License Key Here>", typeof(CartItemsMapping).Assembly);


// Add httpclient for internal service communcation 

builder.Services.AddHttpClient("MyApiClient");
builder.Services.AddHttpContextAccessor();


//app.MapGet("/", () => "Hello World!");
var app = builder.Build();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ExceptionMiddleWare>();
app.Run();
