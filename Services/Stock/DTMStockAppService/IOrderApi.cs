using System.Text.Json.Serialization;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace DTMStockAppService
{
    [LoggingFilter]
    public interface IOrderApi : IHttpApi
    {
        [HttpGet("api/Order/GenerateOrderNo")]
        ITask<string> GenerateOrderNoAsync(CancellationToken cancellationToken = default);

        [HttpPost("api/Order/CreateOrder")]
        Task CreateOrderAsync([AliasAs("OrderNo")] string orderNo, CancellationToken cancellationToken = default);

        [HttpGet("api/Order/GetOrder")]
        ITask<OrderInfo> GetOrderAsync([AliasAs("OrderNo")] string orderNo, CancellationToken cancellationToken = default);

        [HttpPost("api/Order/DeleteOrder")]
        Task DeleteOrderAsync([AliasAs("OrderNo")] string orderNo, CancellationToken cancellationToken = default);

    }

    public class OrderInfo
    {
        [JsonPropertyName("orderNo")]
        public string OrderNo { get; set; }

        [JsonPropertyName("productNo")]
        public string ProductNo { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

    }
}