using System;
using System.ComponentModel.DataAnnotations;

namespace StocksApi.Controllers
{
    public class SavePortfolioDto : BaseDto
    {
        [Required]
        public string Name { get; set; }

        public string HolderIdentificationNumber { get; set; }
    }
}