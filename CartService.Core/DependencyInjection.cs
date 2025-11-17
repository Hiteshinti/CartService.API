using Microsoft.Extensions.DependencyInjection;


namespace CartService.Core
{
    public static class DependencyInjection
    {

          public static IServiceCollection AddCore(this IServiceCollection services)
          {
            services.AddTransient<ICartService, CartService>();
            return services;
          }
    }
}
