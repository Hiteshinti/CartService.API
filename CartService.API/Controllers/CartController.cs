using CartService.Core;
using CartService.Core.Dto;
using CartService.Core.IProviders;
using Microsoft.AspNetCore.Mvc;

namespace CartService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController:ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IUserProvider _userProvider;
        private readonly ILogger<CartController> _logger;
        public CartController (
        ICartService cartService, 
        IUserProvider userProvider,
        ILogger<CartController> logger) 
        { 
            _cartService = cartService;
            _userProvider = userProvider;   
            _logger = logger;   

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
        public async Task<IActionResult> GetCartItemsById(string userId)
        {
             var Userid= User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("userId for Getting caritems" + userId);
            CartResponseDto? cartResponseDto =  await _cartService.GetItems(Guid.Parse(userId));
            return Ok(cartResponseDto);

        }

    }
}
