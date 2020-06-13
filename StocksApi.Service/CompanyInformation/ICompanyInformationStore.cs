using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

namespace StocksApi.Service.Stock
{
    public interface ICompanyInformationStore
    {
        Task<string> GetFromStore();
    }
}