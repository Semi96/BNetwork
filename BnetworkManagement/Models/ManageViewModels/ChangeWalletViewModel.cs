using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BnetworkManagement.Models.ManageViewModels
{
    public class ChangeWalletViewModel
    {
        [Required]
        [Display(Name = "Wallet Public Key")]
        [StringLength(200)]
        public string WalletKey { get; set; }
        [Required]
        [Display(Name = "Confirm Wallet Public Key")]
        [StringLength(200)]
        public string ConfirmWalletKey { get; set; }
        [Required]
        [Display(Name = "Wallet Currency")]
        public Currencies Currency { get; set; }

        public bool DoesWalletExist { get; set; }

        public string StatusMessage { get; set; }

        [Required]
        [Display(Name = "New Wallet Public Key")]
        [StringLength(200)]
        public string NewWalletKey { get; set; }
        [Required]
        [Display(Name = "New Wallet Currency")]
        public Currencies NewCurrency { get; set; }

    }
}
