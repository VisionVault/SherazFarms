using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class COAVM
    {
        public bool IsCTLChanged { get; set; }
        public string CTLCode { get; set; }
        public string CTL { get; set; }
        public bool IsGALChanged { get; set; }
        public string GALCode { get; set; }
        public string GAL { get; set; }
        public string AccCode { get; set; }
        public string Code { get; set; }
        public string Acc { get; set; }
    }
}
