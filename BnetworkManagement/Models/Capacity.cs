using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BnetworkManagement.Models
{
    public class Capacity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CapacityId { get; set; }
        [ConcurrencyCheck]
        public int TotalMegaHashAvailable { get; set; }
        public int UnitPrice { get; set; }


    }
}
