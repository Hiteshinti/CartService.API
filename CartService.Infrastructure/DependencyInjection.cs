using CartService.Core;
using CartService.Infrastructure.DbContext;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.Infrastructure
{
    public static class DependencyInjection
    {

         public static IServiceCollection AddInfraStructure(this IServiceCollection services)
         {
            services.AddScoped<DapperDbContext>();
            services.AddTransient<ICartRepository, CartRepository>();
            return services;
         }

    }
}
