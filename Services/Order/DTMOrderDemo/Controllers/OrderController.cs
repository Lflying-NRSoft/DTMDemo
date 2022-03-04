using DBHelper;
using Dtmcli;
using DTMOrderAppService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DTMOrderDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IBranchBarrierFactory _barrierFactory;
        private readonly IOrderAppService _orderAppService;

        public OrderController(
            IBranchBarrierFactory barrierFactory,
            IOrderAppService orderAppService,
            ILogger<OrderController> logger)
        {
            _logger = logger;
            _barrierFactory = barrierFactory;
            _orderAppService = orderAppService;
        }

        /// <summary>
        /// 生成唯一的订单号
        /// </summary>
        /// <returns></returns>
        [HttpGet("GenerateOrderNo")]
        public Task<string> GenerateOrderNo()
        {
            var orderNo = Guid.NewGuid().ToString();
            _logger.LogInformation($"新产生的订单号为：{orderNo}");
            return Task.FromResult(orderNo);
        }

        /// <summary>
        /// 生成订单
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(string OrderNo)
        {
            var result = await _orderAppService.CreateOrder(OrderNo);
            return Ok(new { dtm_result = "SUCCESS" });
        }

        /// <summary>
        /// 查询指定订单号的订单信息
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        [HttpGet("GetOrder")]
        public async Task<OrderInfo> GetOrder(string OrderNo)
        {
            return await _orderAppService.GetOrder(OrderNo);
        }

        /// <summary>
        /// 删除指定订单号的订单信息
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        [HttpPost("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(string OrderNo)
        {
            await _orderAppService.UpdateOrder(OrderNo, "Invalid");
            return Ok(new { dtm_result = "SUCCESS" });
        }

        /// <summary>
        /// 生成订单
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        [HttpPost("BarrierCreateOrder")]

        public async Task<IActionResult> BarrierCreateOrder([FromBody] string OrderNo)
        {
            var barrier = _barrierFactory.CreateBranchBarrier(Request.Query);
            using (var conn = Db.GeConn())
            {
                await barrier.Call(conn, async (tx) =>
                {
                    await _orderAppService.CreateOrder(OrderNo, null, tx);
                });
            }

            return Ok(new { dtm_result = "SUCCESS" });
        }

        [HttpPost("BarrierCreateOrderRevert")]
        public IActionResult BarrierCreateOrderRevert([FromQuery] string gid, [FromQuery] string trans_type,
            [FromQuery] string branch_id, [FromQuery] string branch_type, [FromBody] string OrderNo)
        {
            _logger.LogInformation("BarrierCreateOrderRevert, QueryString={0}", Request.QueryString);
            _logger.LogInformation($"订单要取消掉了哦！{OrderNo}");
            return Ok(new { dtm_result = "SUCCESS" });
        }


        #region TCC

        [HttpPost("TryBarrierCreateOrder")]
        public async Task<IActionResult> TryBarrierCreateOrder([FromBody] string OrderNo)
        {
            var barrier = _barrierFactory.CreateBranchBarrier(Request.Query);
            using (var conn = Db.GeConn())
            {
                await barrier.Call(conn, async (tx) =>
                {
                    await _orderAppService.CreateOrder(OrderNo, "Draft", tx);
                });
            }

            return Ok(new { dtm_result = "SUCCESS" });
        }

        [HttpPost("ConfirmBarrierCreateOrder")]
        public async Task<IActionResult> ConfirmBarrierCreateOrder([FromBody] string OrderNo)
        {
            var barrier = _barrierFactory.CreateBranchBarrier(Request.Query);
            using (var conn = Db.GeConn())
            {
                await barrier.Call(conn, async (tx) =>
                {
                    await _orderAppService.UpdateOrder(OrderNo, "Completed", tx);
                });
            }

            return Ok(new { dtm_result = "SUCCESS" });
        }

        [HttpPost("CancelBarrierCreateOrder")]
        public async Task<IActionResult> CancelBarrierCreateOrder([FromBody] string OrderNo)
        {
            var barrier = _barrierFactory.CreateBranchBarrier(Request.Query);
            using (var conn = Db.GeConn())
            {
                await barrier.Call(conn, async (tx) =>
                {
                    await _orderAppService.UpdateOrder(OrderNo, "Invalid", tx);
                });
            }

            return Ok(new { dtm_result = "SUCCESS" });
        }
        #endregion  
    }
}
