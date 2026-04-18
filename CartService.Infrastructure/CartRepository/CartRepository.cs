using CartService.Core;
using CartService.Core.Constants;
using CartService.Core.Entities;
using CartService.Core.Helpers;
using CartService.Infrastructure.DbContext;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Infrastructure
{
    public class CartRepository: ICartRepository
    {
        private readonly DapperDbContext _dbcontext;
        public CartRepository(DapperDbContext dbContext) 
        { 

            _dbcontext = dbContext;
        }
        public async Task<Cart?> AddItemsToCart(Cart cartItems)
        {

            var param = new DynamicParameters();
            param.Add("@CartId", cartItems.CartId);
            param.Add("@UserId", cartItems.UserId);
            param.Add("@items", cartItems.items.ToSqlRecords().AsTableValuedParameter("dbo.ItemTableType"));
              

            using (var result = await _dbcontext.DbConnection.QueryMultipleAsync(Constants.InsertCartItem, param, commandType:CommandType.StoredProcedure))
            {
                var cart = await result.ReadFirstOrDefaultAsync<Cart>();
                var items = await result.ReadAsync<Items>();

                if (cart == null)
                    return null;

                cart.items = items.ToList();
                return cart;
            }
             
        }

        public async Task<Cart?> GetItemsFromCart(Guid userId)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", userId);

            using (var result = await _dbcontext.DbConnection.QueryMultipleAsync(Constants.GetCartItems, param, commandType: CommandType.StoredProcedure))
            {
                var cart = await result.ReadFirstOrDefaultAsync<Cart>();
                var items = await result.ReadAsync<Items>();

                if (cart == null)
                    return null;

                cart.items = items.ToList();
                return cart;
            }
        }
    }
}
