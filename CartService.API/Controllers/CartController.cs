using CartService.Core;
using CartService.Core.Dto;
using CartService.Core.IProviders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CartService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController:ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IUserProvider _userProvider;
        private readonly ILogger<CartController> _logger;
        private readonly IDistributedCache _cache;
        public CartController (
        ICartService cartService, 
        IUserProvider userProvider,
        ILogger<CartController> logger,
        IDistributedCache cache) 
        { 
            _cartService = cartService;
            _userProvider = userProvider;   
            _logger = logger;   
            _cache=cache;   

        }

        [HttpPost("AddCartItems")]
        public async Task<IActionResult> AddCartItems([FromBody]List<CartItemDto> cartItemDto)
        {
       
            string?authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            _logger.LogInformation("token for logged in user:" + authHeader);
            if (cartItemDto == null || authHeader==null)
                return BadRequest();

            var userId = await _userProvider.ValidateUser(authHeader);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            _logger.LogInformation("userId for token:" + authHeader);
            CartResponseDto? cartResponseDto =  await _cartService.AddItems(cartItemDto,Guid.Parse(userId));
            return Ok(cartResponseDto); 
            
        }

        [HttpGet("GetCartItemsById")]
        public async Task<IActionResult> GetCartItemsById()
        {
            string? authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            _logger.LogInformation("token for logged in user:" + authHeader);

            var userId = await _userProvider.ValidateUser(authHeader);
            if (string.IsNullOrEmpty(userId))
                  return Unauthorized();

            _logger.LogInformation("userId for Getting caritems" + userId);
            var cachedValue = await _cache.GetStringAsync(userId);
           
            CartResponseDto? cartResponseDto = string.IsNullOrEmpty(cachedValue)
                ? await _cartService.GetItems(Guid.Parse(userId))
                : JsonSerializer.Deserialize<CartResponseDto>(cachedValue);

            if (string.IsNullOrEmpty(cachedValue))
            {
                await _cache.SetStringAsync(
                    userId,
                    JsonSerializer.Serialize(cartResponseDto));
            }

            return Ok(cartResponseDto);

        }

    }
}
