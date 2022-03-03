using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTMStockAppService
{
    public interface IStockAppService
    {
        Task<bool> UpdateStock(string orderNo, DbTransaction tran = null);
    }
}
