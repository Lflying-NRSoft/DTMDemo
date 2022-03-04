using DBHelper;
using Dtmcli;
using DTMStockAppService;
using Microsoft.AspNetCore.Mvc;

namespace DTMStockDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ILogger<StockController> _logger;
        private readonly IBranchBarrierFactory _barrierFactory;
        private readonly IStockAppService _stockAppService;

        public StockController(
            IBranchBarrierFactory barrierFactory,
            IStockAppService stockAppService,
            ILogger<StockController> logger)
        {
            _logger = logger;
            _barrierFactory = barrierFactory;
            _stockAppService = stockAppService;
        }

        /// <summary>
        /// 更新库存
        /// </summary>
        /// <param name="productNo"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpPost("UpdateStock")]
        public Task<bool> UpdateStock(string orderNo)
        {
            return _stockAppService.UpdateStock(orderNo);
        }

        /// <summary>
        /// 更新库存-子事务屏障
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <returns></returns>
        [HttpPost("BarrierUpdateStock")]
        public async Task<IActionResult> BarrierUpdateStock([FromBody] string orderNo)
        {
            var barrier = _barrierFactory.CreateBranchBarrier(Request.Query);
            using var db = Db.GeConn();
            await barrier.Call(db, async (tx) =>
            {
                await _stockAppService.UpdateStock(orderNo, StockType.Normal, tx);
            });

            return Ok(new { dtm_result = "SUCCESS" });
        }

        [HttpPost("BarrierUpdateStockRevert")]
        public IActionResult BarrierUpdateStockRevert([FromQuery] string gid, [FromQuery] string trans_type,
            [FromQuery] string branch_id, [FromQuery] string branch_type, [FromBody] string OrderNo)
        {
            _logger.LogInformation("BarrierUpdateStockRevert, QueryString={0}", Request.QueryString);
            _logger.LogInformation($"库存要反扣减！{OrderNo}");
            return Ok(new { dtm_result = "SUCCESS" });
        }

        #region TCC

        [HttpPost("TryBarrierUpdateStock")]
        public async Task<IActionResult> TryBarrierUpdateStock([FromBody] string orderNo)
        {
            var barrier = _barrierFactory.CreateBranchBarrier(Request.Query);
            using var db = Db.GeConn();
            await barrier.Call(db, async (tx) =>
            {
                await _stockAppService.UpdateStock(orderNo, StockType.Try, tx);
            });

            return Ok(new { dtm_result = "SUCCESS" });
        }

        [HttpPost("ConfirmBarrierUpdateStock")]
        public async Task<IActionResult> ConfirmBarrierUpdateStock([FromBody] string orderNo)
        {
            var barrier = _barrierFactory.CreateBranchBarrier(Request.Query);
            using var db = Db.GeConn();
            await barrier.Call(db, async (tx) =>
            {
                await _stockAppService.UpdateStock(orderNo, StockType.Confirm, tx);
            });

            return Ok(new { dtm_result = "SUCCESS" });
        }

        [HttpPost("CancelBarrierUpdateStock")]
        public async Task<IActionResult> CancelBarrierUpdateStock([FromBody] string orderNo)
        {
            var barrier = _barrierFactory.CreateBranchBarrier(Request.Query);
            using var db = Db.GeConn();
            await barrier.Call(db, async (tx) =>
            {
                await _stockAppService.UpdateStock(orderNo, StockType.Cancel, tx);
            });

            return Ok(new { dtm_result = "SUCCESS" });
        }

        #endregion
    }
}
