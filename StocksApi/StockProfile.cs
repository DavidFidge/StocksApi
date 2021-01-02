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
            CreateMap<Holding, HoldingDto>();

            CreateMap<SavePortfolioDto, Portfolio>();
            CreateMap<Portfolio, PortfolioDto>();

            CreateMap<SaveEndOfDayDto, EndOfDay>();
            CreateMap<EndOfDay, EndOfDayDto>();

            CreateMap<SaveStockDto, Stock>();
            CreateMap<Stock, StockDto>();
            
            CreateMap<SaveDividendDto, Dividend>();
            CreateMap<Dividend, DividendDto>();
        }
    }
}