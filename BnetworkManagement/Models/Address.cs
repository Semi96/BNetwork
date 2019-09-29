using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BnetworkManagement.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string Addressline1 { get; set; }
        public string Addressline2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

    }
}
