using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BnetworkManagement.Models
{
    public class RentalPurchaseContract
    {
        [Key]
        public int TransactionId { get; set; }
        public ApplicationUser AppUser { get; set;}
        public DateTime PurchaseDate { get; set; }
        public int MHAmount { get; set; }
        public decimal UnitPrice { get; set; }
        public DiscountCode DiscountCode { get; set; }
        public double Taxrate { get; set; }
        public string PaymentToken { get; set; }
        public Currencies Currency { get; set; }
        public MiningInventory Batch { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
