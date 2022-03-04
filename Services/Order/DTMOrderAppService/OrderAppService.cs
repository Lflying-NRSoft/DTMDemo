using System.Data;

namespace DTMOrderAppService
{
    public class OrderAppService : IOrderAppService
    {

        public async Task<bool> CreateOrder(string OrderNo, string status = null, IDbTransaction tran = null)
        {
            return await DBHelper.Db.CreateSaleOrder(OrderNo, "Product001", "商品001", 5, status, tran);
        }

        public async Task<bool> UpdateOrder(string OrderNo, string status, IDbTransaction tran = null)
        {
            return await DBHelper.Db.UpdateSaleOrder(OrderNo, status, tran);
        }

        public async Task<OrderInfo> GetOrder(string OrderNo)
        {
            var data = await DBHelper.Db.GetOrder(OrderNo);

            if (data != null)
            {
                return new OrderInfo
                {
                    OrderNo = data.ORDER_NO,
                    ProductNo = data.PRODUCT_NO,
                    Count = data.COUNT,
                };
            }

            return null;
        }
    }
}
