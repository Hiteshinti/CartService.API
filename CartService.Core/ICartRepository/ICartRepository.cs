using CartService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Core
{
    public interface ICartRepository
    {
        Task<Cart?> AddItemsToCart(Cart cart);
        Task<Cart?> GetItemsFromCart(Guid userId);
    }
}
