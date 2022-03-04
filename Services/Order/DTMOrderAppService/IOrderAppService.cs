using System.Data;

namespace DTMOrderAppService
{
    public interface IOrderAppService
    {
        Task<bool> CreateOrder(string OrderNo, string status = null, IDbTransaction tran = null);

        Task<bool> UpdateOrder(string OrderNo, string status, IDbTransaction tran = null);

        Task<OrderInfo> GetOrder(string OrderNo);
    }
}
