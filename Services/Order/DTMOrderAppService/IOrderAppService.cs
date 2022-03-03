using System.Data;

namespace DTMOrderAppService
{
    public interface IOrderAppService
    {
        Task<bool> CreateOrder(string OrderNo, IDbTransaction tran = null);

        Task<OrderInfo> GetOrder(string OrderNo);
    }
}
