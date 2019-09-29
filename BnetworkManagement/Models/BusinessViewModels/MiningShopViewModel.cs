using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BnetworkManagement.Models.BusinessViewModels
{
    public class MiningShopViewModel
    {
        [Display(Name = "Available Capacity")]
  
        public int TotalMegaHashAvailable { get; set; }

        [Display(Name = "Minable Currencies")]
       
        public Currencies MineableCurrencies { get; set; }

        [Display(Name = "Price per MH/s")]
     
        public decimal MegaHashPriceRate { get; set; }
    }
}
