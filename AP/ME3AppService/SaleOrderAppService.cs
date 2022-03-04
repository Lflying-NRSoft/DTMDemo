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
        private readonly TccGlobalTransaction _globalTransaction;
        private readonly ILogger<SaleOrderAppService> _logger;

        public SaleOrderAppService(
            IOrderApi orderApi,
            IStockApi stockApi,
            IDtmClient dtmClient,
            IDtmTransFactory transFactory,
            TccGlobalTransaction transaction,
            ILogger<SaleOrderAppService> logger)
        {
            _orderApi = orderApi;
            _stockApi = stockApi;
            _dtmClient = dtmClient;
            _transFactory = transFactory;
            _globalTransaction = transaction;
            _logger = logger;
        }

        public async Task<string> CreateSaleOrder()
        {
            // 1. 获取订单号
            var orderNo = await _orderApi.GenerateOrderNoAsync();

            // 2. 创建订单
            await _orderApi.CreateOrderAsync(orderNo);

            // 3. 扣减库存
            var result = await _stockApi.UpdateStockAsync(orderNo);

            // 4. 发送消息刷新缓存

            _logger.LogInformation($"串行逻辑执行结果为：订单号：{orderNo}，库存扣减结果：{result}");

            return orderNo;
        }

        /// <summary>
        /// 创建销售订单-DTM分布式事务（Saga）
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> SagaCreateSaleOrder(CancellationToken cancellationToken)
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

        /// <summary>
        /// 创建销售订单-DTM分布式事务（TCC模式）
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> TCCCreateSaleOrder(CancellationToken cancellationToken)
        {
            try
            {
                await _globalTransaction.Excecute(async (tcc) =>
                {
                    // 1. 获取订单号
                    var orderNo = await _orderApi.GenerateOrderNoAsync();

                    // 2. 创建订单
                    var res1 = await tcc.CallBranch(orderNo,
                        "http://localhost:32004/api/Order/TryBarrierCreateOrder",
                        "http://localhost:32004/api/Order/ConfirmBarrierCreateOrder",
                        "http://localhost:32004/api/Order/CancelBarrierCreateOrder",
                        cancellationToken);

                    // 3. 扣减库存
                    var res2 = await tcc.CallBranch(orderNo,
                        "http://localhost:32007/api/Stock/TryBarrierUpdateStock",
                        "http://localhost:32007/api/Stock/ConfirmBarrierUpdateStock",
                        "http://localhost:32007/api/Stock/CancelBarrierUpdateStock",
                        cancellationToken);

                    _logger.LogInformation($"tcc returns: {res1}-{res2}");

                }, cancellationToken);

                // 4. 发送消息刷新缓存

                return "SUCCESS";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TccBarrier Error");
                return "FAILURE";
            }
        }
    }
}
