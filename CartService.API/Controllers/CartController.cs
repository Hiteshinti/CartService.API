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
        public CartController (
        ICartService cartService, 
        IUserProvider userProvider) 
        { 
            _cartService = cartService;
            _userProvider = userProvider;   

        }

        [HttpPost("AddCartItems")]
        public async Task<IActionResult> AddCartItems([FromBody]List<CartItemDto> cartItemDto)
        {
            string?authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (cartItemDto == null || authHeader==null)
                return BadRequest();

            var userId = await _userProvider.ValidateUser(authHeader);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
                   
            CartResponseDto? cartResponseDto =  await _cartService.AddItems(cartItemDto,Guid.Parse(userId));
            return Ok(cartResponseDto); 
            
        }
    }
}
