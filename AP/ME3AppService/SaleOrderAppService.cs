using Dtmcli;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore;

namespace ME3AppService
{
    public class SaleOrderAppService : ISaleOrderAppService
    {
        private readonly IOrderApi _orderApi;
        private readonly IStockApi _stockApi;
        private readonly IDtmClient _dtmClient;
        private readonly IDtmTransFactory _transFactory;
        private readonly ILogger<SaleOrderAppService> _logger;

        public SaleOrderAppService(
            IOrderApi orderApi,
            IStockApi stockApi,
            IDtmClient dtmClient,
            IDtmTransFactory transFactory,
            ILogger<SaleOrderAppService> logger)
        {
            _orderApi = orderApi;
            _stockApi = stockApi;
            _dtmClient = dtmClient;
            _transFactory = transFactory;
            _logger = logger;
        }

        public async Task<string> CreateSaleOrder()
        {
            // 1. 获取订单号
            var orderNo = await _orderApi.GenerateOrderNoAsync();

            // 2. 创建订单
            await _orderApi.CreateOrderAsync(orderNo);

            // 3. 扣减库存
            await _stockApi.UpdateStockAsync(orderNo);

            // 4. 发送消息刷新缓存


            return orderNo;
        }

        /// <summary>
        /// 创建销售订单
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> DTMCreateSaleOrder(CancellationToken cancellationToken)
        {
            // 1. 获取订单号
            var orderNo = await _orderApi.GenerateOrderNoAsync();
            var request = new { orderNo = orderNo };

            // 发起分布式事务
            var gid = await _dtmClient.GenGid(cancellationToken);
            var saga = _transFactory.NewSaga(gid)
                // 2. 创建订单
                .Add("http://localhost:32004/api/Order/BarrierCreateOrder", "http://localhost:32004/api/Order/BarrierCreateOrderRevert", orderNo)
                // 3. 扣减库存
                .Add("http://localhost:32007/api/Stock/BarrierUpdateStock", "http://localhost:32007/api/Stock/BarrierUpdateStockRevert", orderNo)
                ;

            await saga.Submit(cancellationToken);

            _logger.LogInformation("分布式事务的全局ID gid is {0}", gid);

            // 4. 发送消息刷新缓存

            return orderNo;
        }
    }
}
