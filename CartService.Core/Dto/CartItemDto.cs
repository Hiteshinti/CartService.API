using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Core.Dto
{
    public  class CartItemDto
    {

        public string ItemId { get; set; }
        public int? Quantity { get; set; }
        public string ItemName {  get; set; } 
        public double? Price { get; set; } 

    }

}
