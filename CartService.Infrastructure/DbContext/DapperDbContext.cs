using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Infrastructure.DbContext
{
    public  class DapperDbContext
    {

        private readonly IConfiguration _configuration;
        private readonly IDbConnection _connection;
         public DapperDbContext(IConfiguration configuration)
        {
              _configuration = configuration;
              string? constr = _configuration.GetConnectionString("DefaultConnection");
              _connection = new SqlConnection(constr);
        }

        public IDbConnection DbConnection => _connection;
    }
}
