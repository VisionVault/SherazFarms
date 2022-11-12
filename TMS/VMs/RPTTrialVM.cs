using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class RPTTrialVM
    {
        public string Type { get; set; }
        public bool IsCTLChanged { get; set; }
        public string CTLCode { get; set; }
        public string CTL { get; set; }
        public bool IsGALChanged { get; set; }
        public string GALCode { get; set; }
        public string GAL { get; set; }
        public long AccountId { get; set; }
        public string AccCode { get; set; }
        public string Code { get; set; }
        public string Acc { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public double OpDebit { get; set; }
        public double OpCredit { get; set; }
        public double CurrDebit { get; set; }
        public double CurrCredit { get; set; }
        public double ClDebit { get; set; }
        public double ClCredit { get; set; }
        public double Balance { get; set; }
    }
}
