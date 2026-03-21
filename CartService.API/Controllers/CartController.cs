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
        public async Task<IActionResult> AddCartItems([FromBody] List<CartItemDto> cartItemDto, CancellationToken cancellationToken)
        {
            var validationErrors = ValidateCartItems(cartItemDto);
            if (validationErrors.Count > 0)
            {
                return ValidationProblem(validationErrors);
            }

            var authHeader = HttpContext.Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(authHeader))
            {
                return Unauthorized("Authorization header is required.");
            }

            var userId = await _userProvider.ValidateUser(authHeader, cancellationToken);
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            CartResponseDto? cartResponseDto =  await _cartService.AddItems(cartItemDto, userId.Value);
            return cartResponseDto is null ? StatusCode(StatusCodes.Status502BadGateway) : Ok(cartResponseDto);

        }

        private static Dictionary<string, string[]> ValidateCartItems(List<CartItemDto> cartItems)
        {
            var errors = new Dictionary<string, string[]>();

            if (cartItems is null || cartItems.Count == 0)
            {
                errors["cartItems"] = ["At least one cart item is required."];
                return errors;
            }

            var itemErrors = new List<string>();
            for (var index = 0; index < cartItems.Count; index++)
            {
                var item = cartItems[index];
                if (item is null)
                {
                    itemErrors.Add($"Item at index {index} is required.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(item.ItemId) || !Guid.TryParse(item.ItemId, out _))
                {
                    itemErrors.Add($"Item at index {index} must contain a valid item id.");
                }

                if (string.IsNullOrWhiteSpace(item.ItemName))
                {
                    itemErrors.Add($"Item at index {index} must contain an item name.");
                }

                if (!item.Quantity.HasValue || item.Quantity <= 0)
                {
                    itemErrors.Add($"Item at index {index} must have a quantity greater than zero.");
                }

                if (!item.Price.HasValue || item.Price < 0)
                {
                    itemErrors.Add($"Item at index {index} must have a non-negative price.");
                }
            }

            if (itemErrors.Count > 0)
            {
                errors["cartItems"] = itemErrors.ToArray();
            }

            return errors;
        }
    }
}
