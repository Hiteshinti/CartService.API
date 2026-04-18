using CartService.Core.Dto;
using CartService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Core.Mappers
{
    public class CartItemsMapping :AutoMapper.Profile
    {

        public CartItemsMapping() 
        {


            CreateMap<CartItemDto, Items>();
            // map collection -> Cart
            CreateMap<IEnumerable<CartItemDto>, Cart>()
                .ForMember(dest => dest.CartId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.items, opt => opt.MapFrom(src => src));

            CreateMap<Cart, CartResponseDto>()
                .ForMember(dest => dest.CartId, opt => opt.MapFrom(opt => opt.CartId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(opt => opt.UserId))
                .ForMember(dest => dest.CartItems, opt => opt.MapFrom(opt => opt.items.Select(i => new CartItemDto
                {
                    ItemId = i.ItemId.ToString(),
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToArray()))
                .ForMember(dest => dest.success, opt => opt.MapFrom(_ => true));    
        }   
    }
}
