using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class RPTLedgerVM
    {
        public DateTime TransactionDate { get; set; }
        public string Date { get; set; }
        public long DocId { get; set; }
        public string DocType { get; set; }
        public string Account { get; set; }
        public string Narration { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public double Balance { get; set; }
    }
}
