using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class RPTDyeVM
    {
        public long DocId { get; set; }
        public string DocType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Date { get; set; }
        public string Account { get; set; }
        public string LotNumber { get; set; }
        public string Product { get; set; }
        public double Qty { get; set; }
        public double Rate { get; set; }
        public double Gain { get; set; }
        public int? RollNumber { get; set; }
        public double Loss { get; set; }
        public string Type { get; set; }
    }
}
