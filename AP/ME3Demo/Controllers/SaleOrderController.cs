using Dtmcli;
using ME3AppService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ME3Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrderController : ControllerBase
    {
        private readonly ILogger<SaleOrderController> _logger;
        private readonly IBranchBarrierFactory _barrierFactory;
        private readonly ISaleOrderAppService _saleOrderAppService;

        public SaleOrderController(
            IBranchBarrierFactory barrierFactory,
            ISaleOrderAppService saleOrderAppService,
            ILogger<SaleOrderController> logger)
        {
            _logger = logger;
            _barrierFactory = barrierFactory;
            _saleOrderAppService = saleOrderAppService;
        }

        /// <summary>
        /// 创建销售订单
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        public async Task<string> Create()
        {
            return await _saleOrderAppService.CreateSaleOrder();
        }

        /// <summary>
        /// 创建销售订单-DTM分布式事务（Saga）
        /// </summary>
        /// <returns></returns>
        [HttpPost("SagaCreate")]
        public async Task<string> SagaCreate(CancellationToken cancellationToken)
        {
            return await _saleOrderAppService.SagaCreateSaleOrder(cancellationToken);
        }

        /// <summary>
        /// 创建销售订单-DTM分布式事务（TCC模式）
        /// </summary>
        /// <returns></returns>
        [HttpPost("TCCCreate")]
        public async Task<string> TCCCreate(CancellationToken cancellationToken)
        {
            return await _saleOrderAppService.TCCCreateSaleOrder(cancellationToken);
        }
    }
}
