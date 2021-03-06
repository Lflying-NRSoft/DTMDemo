using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace DBHelper
{
    public class Db
    {
        private static readonly string _conn = "Server=192.168.148.16;Database=ME3Demo;User Id=sa;Password=App1234;TrustServerCertificate=true";

        public static DbConnection GeConn() => new SqlConnection(_conn);

        public static async Task<bool> CreateSaleOrder(string orderNo, string productNo, string productName, int count, string status = null, IDbTransaction tran = null)
        {
            try
            {
                var sql = @"INSERT INTO [Sale_Order] ([ORDER_NO],[PRODUCT_NO],[PRODUCT_NAME],[COUNT],[STATUS],[CREATE_DATE])
                    VALUES(@ORDER_NO,@PRODUCT_NO,@PRODUCT_NAME,@COUNT,@STATUS,@CREATE_DATE)";
                if (tran != null)
                {
                    var conn = tran.Connection;
                    var affectedRows = await conn.ExecuteAsync(sql, new
                    {
                        ORDER_NO = orderNo,
                        PRODUCT_NO = productNo,
                        PRODUCT_NAME = productName,
                        COUNT = count,
                        STATUS = status ?? "Completed",
                        CREATE_DATE = DateTime.Now
                    }, transaction: tran);
                    return affectedRows > 0;
                }
                else
                {
                    using var conn = GeConn();
                    await conn.OpenAsync();
                    var affectedRows = await conn.ExecuteAsync(sql, new
                    {
                        ORDER_NO = orderNo,
                        PRODUCT_NO = productNo,
                        PRODUCT_NAME = productName,
                        COUNT = count,
                        STATUS = status ?? "Completed",
                        CREATE_DATE = DateTime.Now
                    });
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<bool> UpdateSaleOrder(string orderNo, string status, IDbTransaction tran = null)
        {
            try
            {
                var sql = @"UPDATE [Sale_Order] set STATUS =@STATUS where ORDER_NO = @ORDER_NO";
                if (tran != null)
                {
                    var conn = tran.Connection;
                    var affectedRows = await conn.ExecuteAsync(sql, new
                    {
                        ORDER_NO = orderNo,
                        STATUS = status ?? "Completed",
                    }, transaction: tran);
                    return affectedRows > 0;
                }
                else
                {
                    using var conn = GeConn();
                    await conn.OpenAsync();
                    var affectedRows = await conn.ExecuteAsync(sql, new
                    {
                        ORDER_NO = orderNo,
                        STATUS = status ?? "Completed",
                    });
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static async Task<dynamic> GetOrder(string orderNo)
        {
            try
            {
                var sql = @"Select ORDER_NO,PRODUCT_NO,COUNT From [Sale_Order] Where ORDER_NO=@orderNo";
                using var conn = GeConn();
                await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync(sql, new
                {
                    orderNo = orderNo
                });
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<bool> UpdateStock(string productNo, int count, DbTransaction tran = null)
        {
            try
            {
                var sql = "update Stock set STOCK_COUNT = STOCK_COUNT - @count where ProductNo = @productNo";
                if (tran != null)
                {
                    var conn = tran.Connection;
                    var affectedRows = await conn.ExecuteAsync(sql, new
                    {
                        count = count,
                        productNo = productNo
                    }, tran);
                    return affectedRows > 0;
                }
                else
                {
                    using var conn = GeConn();
                    var affectedRows = await conn.ExecuteAsync(sql, new
                    {
                        count = count,
                        productNo = productNo
                    });
                    return affectedRows > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<bool> TryReduceStock(string productNo, int count, DbTransaction tran = null)
        {
            try
            {
                var sql = "update Stock set FROZEN_STOCK = FROZEN_STOCK + @count where ProductNo = @productNo";
                if (tran != null)
                {
                    var conn = tran.Connection;
                    var affectedRows = await conn.ExecuteAsync(sql, new
                    {
                        count = count,
                        productNo = productNo
                    }, tran);
                    return affectedRows > 0;
                }
                else
                {
                    using var conn = GeConn();
                    var affectedRows = await conn.ExecuteAsync(sql, new
                    {
                        count = count,
                        productNo = productNo
                    });
                    return affectedRows > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<bool> ConfirmReduceStock(string productNo, int count, DbTransaction tran = null)
        {
            try
            {
                var sql = "update Stock set STOCK_COUNT = STOCK_COUNT - @count, FROZEN_STOCK = FROZEN_STOCK - @count where ProductNo = @productNo";
                if (tran != null)
                {
                    var conn = tran.Connection;
                    var affectedRows = await conn.ExecuteAsync(sql, new
                    {
                        count = count,
                        productNo = productNo
                    }, tran);
                    return affectedRows > 0;
                }
                else
                {
                    using var conn = GeConn();
                    var affectedRows = await conn.ExecuteAsync(sql, new
                    {
                        count = count,
                        productNo = productNo
                    });
                    return affectedRows > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<bool> CancelReduceStock(string productNo, int count, DbTransaction tran = null)
        {
            try
            {
                var sql = "update Stock set FROZEN_STOCK = FROZEN_STOCK - @count where ProductNo = @productNo";
                if (tran != null)
                {
                    var conn = tran.Connection;
                    var affectedRows = await conn.ExecuteAsync(sql, new
                    {
                        count = count,
                        productNo = productNo
                    }, tran);
                    return affectedRows > 0;
                }
                else
                {
                    using var conn = GeConn();
                    var affectedRows = await conn.ExecuteAsync(sql, new
                    {
                        count = count,
                        productNo = productNo
                    });
                    return affectedRows > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}