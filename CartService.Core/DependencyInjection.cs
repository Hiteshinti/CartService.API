using Azure.Messaging.ServiceBus;
using CartService.Core.RabbitMQ;
using CartService.Core.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;


namespace CartService.Core
{
    public static class DependencyInjection
    {

          public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
          {
            services.AddSingleton(_ =>
            {
                return new ServiceBusClient(configuration["ServiceBus_Connection"]);
            });
            services.AddSingleton<IServiceBus, ServiceBus.ServiceBus>();
            services.AddSingleton<IConnection>(options =>
            {
                var configuration = options.GetRequiredService<IConfiguration>();
                var factory = new ConnectionFactory()
                {
                    HostName = configuration["RabbitMQ_HOST"],
                    Port = int.Parse(configuration["RabbitMQ_PORT"]),
                    UserName = configuration["RabbitMQ_USER"],
                    Password = configuration["RabbitMQ_PASSWORD"]
                };
                return factory.CreateConnection();
            });
            services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();
            services.AddTransient<ICartService, CartService>();
            return services;
          }
    }
}
