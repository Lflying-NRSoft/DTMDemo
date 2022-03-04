using Dtmcli;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTMStockAppService
{
    public class StockAppService : IStockAppService
    {
        private readonly IOrderApi _orderApi;
        private readonly IBranchBarrierFactory _barrierFactory;

        public StockAppService(
            IOrderApi orderApi,
            IBranchBarrierFactory barrierFactory)
        {
            _orderApi = orderApi;
            _barrierFactory = barrierFactory;
        }

        public async Task<bool> UpdateStock(string orderNo, StockType type = StockType.Normal, DbTransaction tran = null)
        {
            // TODO 获取订单信息
            var order = await _orderApi.GetOrderAsync(orderNo);
            if (order != null)
            {
                if (type == StockType.Normal)
                {
                    return await DBHelper.Db.UpdateStock(order.ProductNo, order.Count, tran);
                }
                else if (type == StockType.Try)
                {
                    return await DBHelper.Db.TryReduceStock(order.ProductNo, order.Count, tran);
                }
                else if (type == StockType.Confirm)
                {
                    return await DBHelper.Db.ConfirmReduceStock(order.ProductNo, order.Count, tran);
                }
                else if (type == StockType.Cancel)
                {
                    return await DBHelper.Db.CancelReduceStock(order.ProductNo, order.Count, tran);
                }
            }

            return false;
        }
    }
}
