using CartService.Core;
using CartService.Core.IProviders;
using CartService.Core.Mappers;
using CartService.Core.MiddleWare;
using CartService.Core.Options;
using CartService.Core.Providers;
using CartService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ApiSettingsOptions>(
    builder.Configuration.GetSection(ApiSettingsOptions.SectionName));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddInfraStructure();
builder.Services.AddCore();
builder.Services.AddHttpClient<IUserProvider, UserProvider>();
builder.Services.AddAutoMapper(typeof(CartItemsMapping).Assembly);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleWare>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
