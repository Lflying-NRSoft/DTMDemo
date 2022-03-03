using System.Data;

namespace DTMOrderAppService
{
    public class OrderAppService : IOrderAppService
    {

        public async Task<bool> CreateOrder(string OrderNo, IDbTransaction tran = null)
        {
            return await DBHelper.Db.CreateSaleOrder(OrderNo, "Product001", "商品001", 5, tran);
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
