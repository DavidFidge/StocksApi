using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace StocksApi.Service
{
    public abstract class BaseService<T>
    {
        protected readonly ILogger<T> Logger;

        protected BaseService(ILogger<T> logger)
        {
            Logger = logger;
        }
    }
}
