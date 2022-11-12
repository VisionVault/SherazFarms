using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class VoucherVM
    {
        public long DocId { get; set; }
        public int DocTypeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime PostDate { get; set; }
        public long AccountId { get; set; }
        public long DebitAcId { get; set; }
        public long CreditAcId { get; set; }
        public string Narration { get; set; }
        public string NarrationDetailed { get; set; }
        public double Amount { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public int TradeFirmId { get; set; }
        public int TransactionStatusId { get; set; }
        public string UserId { get; set; }
    }
}
