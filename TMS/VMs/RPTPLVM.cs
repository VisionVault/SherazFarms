using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class RPTPLVM
    {
        public int Sr { get; set; }
        public bool IsGroupChanged { get; set; }
        public string GAL { get; set; }
        public string Account { get; set; }
        public double Amount { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
    }
}
