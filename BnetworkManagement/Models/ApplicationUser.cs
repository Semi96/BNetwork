using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BnetworkManagement.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public Address UserAddress { get; set; }
        public UserWallet Wallet { get; set; }
        public UserStatus Status { get; set; }
        public DiscountCode AffiliateCode { get; set; }
        //  public List<RentalPurchaseContract> RentalContracts { get; set; }
    }
}
