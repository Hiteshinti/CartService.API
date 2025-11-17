using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Core.Dto
{
     public record CartResponseDto(
         Guid CartId, 
         Guid UserId, 
         bool? success)
    {
         public CartResponseDto():this(default,default,default)
         {

         }
    }
    
}
