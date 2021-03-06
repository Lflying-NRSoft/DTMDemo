using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3AppService
{
    public interface ISaleOrderAppService
    {
        Task<string> CreateSaleOrder();

        Task<string> SagaCreateSaleOrder(CancellationToken cancellationToken);

        Task<string> TCCCreateSaleOrder(CancellationToken cancellationToken);
    }
}
