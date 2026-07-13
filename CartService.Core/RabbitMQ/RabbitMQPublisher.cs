using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Core.RabbitMQ
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly IConfiguration _configuration;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQPublisher> _logger;   
        private readonly IConnection _connection;   
        public RabbitMQPublisher(IConfiguration configuration, IConnection connection)
        {

            _configuration = configuration;
            _connection = connection;


        }
        public void publish<T>(string ruoteKey, T message)
        {
            using var _channel = _connection.CreateModel();  
            // create exchange 
            _channel.ExchangeDeclare(
                 exchange: "order.Exchange", type: 
                 ExchangeType.Direct,durable:true);

            // publish message to exchange

            _channel.BasicPublish(
                exchange: "order.Exchange",
                routingKey: ruoteKey, basicProperties: null, 
                body: Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(message)));   
        }
    }
}
