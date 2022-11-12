using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class RPTProductionVM
    {
        public long DocId { get; set; }
        public string DocType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Date { get; set; }
        public string LotNumber { get; set; }
        public string Product { get; set; }
        public int? RollNumber { get; set; }
        public string Design { get; set; }
        public int? DesignNumber { get; set; }
        public int? JobNumber { get; set; }
        public double Qty { get; set; }
        public double? Packing { get; set; }
        public double RawFabric { get; set; }
        public double PrintedFabric { get; set; }
        public double WhitePass { get; set; }
        public double InkUsageML { get; set; }
        public double InkUsageL { get; set; }
        public string Type { get; set; }
    }
}
