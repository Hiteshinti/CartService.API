using CartService.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Core
{
    public interface ICartService
    {
        Task<CartResponseDto?> AddItems(List<CartItemDto> cartItems, Guid userId);
    }
}
