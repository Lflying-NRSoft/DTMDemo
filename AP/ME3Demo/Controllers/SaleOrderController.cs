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
        /// 创建销售订单-DTM分布式事务
        /// </summary>
        /// <returns></returns>
        [HttpPost("DTMCreate")]
        public async Task<string> DTMCreate(CancellationToken cancellationToken)
        {
            return await _saleOrderAppService.DTMCreateSaleOrder(cancellationToken);
        }
    }
}
