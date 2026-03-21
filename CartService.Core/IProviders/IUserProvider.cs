using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Core.IProviders
{
    public interface IUserProvider
    {
        Task<Guid?> ValidateUser(string token, CancellationToken cancellationToken = default);
    }
}
