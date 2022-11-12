using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class RPTLotHistoryVM
    {
        public long DocId { get; set; }
        public string DocType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Date { get; set; }
        public string Account { get; set; }
        public string Product { get; set; }
        public double Qty { get; set; }
        public string Type { get; set; }
    }
}
