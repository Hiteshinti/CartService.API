using AutoMapper;
using CartService.Core.Dto;
using CartService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Core
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        public CartService(IMapper mapper, ICartRepository cartRepository) 
        { 
            _cartRepository = cartRepository;   
            _mapper = mapper;   
        }    

        public async Task<CartResponseDto?> AddItems(List<CartItemDto> cartItems, Guid userId)
        {
            Cart cart = _mapper.Map<Cart>(cartItems);
            cart.UserId = userId;

            await _cartRepository.AddItemsToCart(cart);
            return _mapper.Map<CartResponseDto>(cart);
        }
        public async Task<CartResponseDto?> GetItems(Guid userId)
        {
            var cart = await _cartRepository.GetItemsFromCart(userId);
            return _mapper.Map<CartResponseDto>(cart);  
        }
    }
}
