using CartService.Core;
using CartService.Core.Dto;
using CartService.Core.IProviders;
using CartService.Core.RabbitMQ;
using CartService.Core.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
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
        //private readonly IRabbitMQPublisher _rabbitMQPublisher;
        private readonly IServiceBus _serviceBus;
        public CartController (
        ICartService cartService, 
        IUserProvider userProvider,
        ILogger<CartController> logger,
        IDistributedCache cache,
        //IRabbitMQPublisher rabbitMQPublisher,
        IServiceBus serviceBus) 
        { 
            _cartService = cartService;
            _userProvider = userProvider;   
            _logger = logger;   
            _cache=cache;
            _serviceBus = serviceBus;
           //_rabbitMQPublisher=rabbitMQPublisher;


        }

        [HttpPost("AddCartItems")]
        public async Task<IActionResult> AddCartItems([FromBody]List<CartItemDto> cartItemDto,string cartId)
        {
       
            string?authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            _logger.LogInformation("token for logged in user:" + authHeader);
            if (cartItemDto == null || authHeader==null)
                return BadRequest();

            var userId = await _userProvider.ValidateUser(authHeader);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            _logger.LogInformation("userId for token:" + authHeader);
            CartResponseDto? cartResponseDto =  await _cartService.AddItems(cartItemDto,Guid.Parse(userId),cartId);
            await _cache.SetStringAsync(userId, JsonSerializer.Serialize(cartResponseDto));
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

            _logger.LogInformation("Cache items {cachedValue}", cachedValue);
           
            CartResponseDto? cartResponseDto = string.IsNullOrEmpty(cachedValue)
                ? await _cartService.GetItems(userId)
                : JsonSerializer.Deserialize<CartResponseDto>(cachedValue);

            _logger.LogInformation("Cart items retrieved for UserId {UserId}. Cart found: {CartFound}",
            userId,
           cartResponseDto != null);

            if (string.IsNullOrEmpty(cachedValue))  
            {
                await _cache.SetStringAsync(
                    userId,
                    JsonSerializer.Serialize(cartResponseDto));
            }

            return Ok(cartResponseDto);

        }

        [HttpPost("CartCheckOut")]
        public async Task<IActionResult> CartCheckOut()
        {
            string? authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            _logger.LogInformation("token for logged in user:" + authHeader);

            var userId = await _userProvider.ValidateUser(authHeader);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var cachedValue = await _cache.GetStringAsync(userId);

            CartResponseDto? cartResponseDto = string.IsNullOrEmpty(cachedValue)
                ? await _cartService.GetItems(userId)
                : JsonSerializer.Deserialize<CartResponseDto>(cachedValue);

            if (cartResponseDto == null || cartResponseDto.CartItems.Count() == 0)
                return BadRequest("Cart is empty");

            var headers = new Dictionary<string, object>
             {
                { "event", "order.create" },
                { "rowCount", 1 }
              };

           await _serviceBus.Publish(headers, new
            {
                OrderId = Guid.NewGuid(),
                UserId = cartResponseDto.UserId,
                Items = cartResponseDto.CartItems
            }); 

            return Ok(cartResponseDto);  
        }

    }
}
