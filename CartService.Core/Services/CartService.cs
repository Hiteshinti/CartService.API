using AutoMapper;
using CartService.Core.Dto;
using CartService.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Core
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;  
        public CartService(IMapper mapper, ICartRepository cartRepository, ILogger<CartService> logger) 
        { 
            _cartRepository = cartRepository;   
            _mapper = mapper;   
            _logger = logger;   
        }    

        public async Task<CartResponseDto?> AddItems(List<CartItemDto> cartItems, Guid userId, string cartId)
        {
            Cart cart = _mapper.Map<Cart>(cartItems);
            cart.UserId = userId;
            cart.CartId = string.IsNullOrEmpty(cartId)? Guid.NewGuid(): Guid.Parse(cartId); 

            await _cartRepository.AddItemsToCart(cart);
            return _mapper.Map<CartResponseDto>(cart);
        }
        public async Task<CartResponseDto?> GetItems(string userId)
        {
            
            var cart = await _cartRepository.GetItemsFromCart(userId);
            
            _logger.LogInformation("Cart Items {cart}", cart);
            return _mapper.Map<CartResponseDto>(cart);  
        }
    }
}
