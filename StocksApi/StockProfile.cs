using AutoMapper;

using StocksApi.Controllers;
using StocksApi.Model;

namespace StocksApi
{
    public class StockProfile : Profile
    {
        public StockProfile()
        {
            CreateMap<SaveHoldingDto, Holding>();
        }
    }
}