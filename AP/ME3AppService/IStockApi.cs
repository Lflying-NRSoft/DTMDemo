using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;
using WebApiClientCore.Parameters;

namespace ME3AppService
{
    [LoggingFilter]
    public interface IStockApi : IHttpApi
    {
        [HttpPost("api/Stock/UpdateStock")]
        ITask<bool> UpdateStockAsync(string orderNo, CancellationToken cancellationToken = default);

    }
}