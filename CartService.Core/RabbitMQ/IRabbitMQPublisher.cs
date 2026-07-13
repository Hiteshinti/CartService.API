using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Core.RabbitMQ
{
     public interface IRabbitMQPublisher
     {
        void publish<T>(string ruoteKey,T message);   
    }
}
