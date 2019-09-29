using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BnetworkManagement.Models
{
    public class DiscountCode
    {
        [Key]
        public int DiscountId { get; set; }
        public string Code { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountAmount { get; set; }
        public int NumTimesUsed { get; set; }
    }
}
