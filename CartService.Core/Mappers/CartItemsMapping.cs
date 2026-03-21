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


            CreateMap<CartItemDto, Items>()
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => Guid.Parse(src.ItemId)));
            // map collection -> Cart
            CreateMap<IEnumerable<CartItemDto>, Cart>()
                .ForMember(dest => dest.CartId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src));

            CreateMap<Cart, CartResponseDto>()
                .ForMember(dest => dest.CartId, opt => opt.MapFrom(opt => opt.CartId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(opt => opt.UserId))
                .ForMember(dest => dest.success, opt => opt.MapFrom(_ => true));
        }   
    }
}
