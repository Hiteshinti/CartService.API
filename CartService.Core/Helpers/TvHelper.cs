using CartService.Core.Entities;
using Microsoft.Data.SqlClient.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Core.Helpers
{
    public static class TvHelper
    {
        public static IEnumerable<SqlDataRecord> ToSqlRecords(this IEnumerable<Items> items)
        {
            var metaData = new[]
            {
                new SqlMetaData("ItemId", System.Data.SqlDbType.UniqueIdentifier),
                new SqlMetaData("ItemName", System.Data.SqlDbType.NVarChar, 255),
                new SqlMetaData("Quantity", System.Data.SqlDbType.Int),
                new SqlMetaData("Price", System.Data.SqlDbType.Float)
            };

            foreach (var item in items)
            {
                var record = new SqlDataRecord(metaData);
                record.SetGuid(0, item.ItemId);
                record.SetString(1, item.ItemName ?? string.Empty);
                record.SetInt32(2, item.Quantity ?? 0);
                record.SetDouble(3, item.Price ?? 0);
                yield return record;
            }
        }
    }
}
