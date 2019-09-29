using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BnetworkManagement.Models
{
    public class UserWallet
    {
        [Key]
        public int WalletId { get; set; }
        public string WalletKey { get; set; }
        public Currencies Currency { get; set; }
    }
}
