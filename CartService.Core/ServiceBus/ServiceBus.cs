using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Core.ServiceBus
{
    public class ServiceBus :IServiceBus
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusSender _serviceBusSender;
        public ServiceBus(ServiceBusClient serviceBusClient)
        {

            _serviceBusClient = serviceBusClient;
            _serviceBusSender = _serviceBusClient.CreateSender("order.create");
        }

        public async Task Publish<T>(Dictionary<string, object> headers, T message)
        {

            var serviceBusMessage = new ServiceBusMessage(System.Text.Json.JsonSerializer.Serialize(message));
            foreach (var header in headers)
            {
                serviceBusMessage.ApplicationProperties.Add(header.Key, header.Value);
            }
            await _serviceBusSender.SendMessageAsync(serviceBusMessage);
        }
    }
}
