using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BnetworkManagement.Models
{
    public class MiningInventory
    {
        [Key]
        public int BatchId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime MiningStartDate { get; set; }
        public DateTime MiningEndDate { get; set; }
        public int MHAmount { get; set; }
        public BatchStatus Status { get; set; }
        public Currencies Currency { get; set; }
        //public List<RentalPurchaseContract> RentalContracts { get; set; }

    }
}
