using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Core.ServiceBus
{
    public interface IServiceBus
    {
        Task Publish<T>(Dictionary<string, object> headers, T message);

    }
}
