using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StocksApi.Model;

namespace StocksApi.Service.EndOfDayData
{
    public interface IEndOfDayStore
    {
        Task<IList<string>> GetFromStore();
    }
}