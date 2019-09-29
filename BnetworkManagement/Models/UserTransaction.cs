using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BnetworkManagement.Models
{
    public class UserTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }
        public ApplicationUser AppUser { get; set; }
        public double CryptoRequested { get; set; } 
        public double CryptoDelivered { get; set; }
        public double FeePaid { get; set; }
        public DateTime Date { get; set; }

    }
}
