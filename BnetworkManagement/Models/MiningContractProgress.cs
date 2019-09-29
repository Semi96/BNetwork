using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BnetworkManagement.Models
{
    public class MiningContractProgress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Timestamp]
        public int ProgressId { get; set; }
        public RentalPurchaseContract RentalPurchaseContract { get; set; }
        public ApplicationUser AppUser { get; set; }
        public double CryptoMined { get; set; }
        public DateTime Date { get; set; }

    }
}
