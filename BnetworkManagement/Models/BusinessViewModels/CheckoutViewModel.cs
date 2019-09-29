using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BnetworkManagement.Models.BusinessViewModels
{
    public class CheckoutViewModel
    {
        [Display(Name = "MH/s")]

        public int MegaHashPurchased { get; set; }

        [Display(Name = "Cryptocurrency")]

        public Currencies Cryptocurrency { get; set; }

        [Display(Name = "Price per MH/s")]
 
        public decimal MegaHashPriceRate { get; set; }

        [Display(Name = "Tax Rate")]
       
        public decimal PurchaseTaxRate { get; set; }

        [Display(Name = "Net Purchase Tax")]

        public decimal NetPurchaseTax { get; set; }

        [Display(Name = "Total Purchase Price")]
      
        public decimal TotalPurchasePrice { get; set; }

        [Display(Name = "No-Tax Purchase Price")]

        public decimal NoTaxPurchasePrice { get; set; }

        [Display(Name = "Discount Code")]
        [StringLength(100)]
        public string DiscountCode { get; set; }

        public bool DiscountApplied { get; set; }

        [Display(Name = "Wallet Public Key")]
        [StringLength(200)]
        public string WalletPublicKey { get; set; }

        [Display(Name = "Stripe payment token")]
        public string PaymentToken { get; set; }

        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }
    }
}
