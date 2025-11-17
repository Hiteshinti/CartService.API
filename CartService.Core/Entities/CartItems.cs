using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Core.Entities
{

    public class Cart
    {
        [Key]
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        public List<Items>? items { get; set; }  

    }
    public class Items
    { 
        public Guid ItemId { get; set; }  
        public string ItemName { get; set; }  
        public int? Quantity { get; set; }  
        public double? Price { get; set; }   

       
    }
}
