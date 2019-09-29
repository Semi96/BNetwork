using BnetworkManagement.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BnetworkManagement.Models.AdminViewModels
{
    public class UsersListViewModel
    {
        public IEnumerable<ApplicationUser> Users {get;set;}
        public PagingInfo PagingInfo { get; set; }
        public int? CurrentStatus { get; set; }
    }
}
