using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class RPTStockVM
    {
        public int? CategoryId { get; set; }
        public string Category { get; set; }
        public long ProductId { get; set; }
        public string Product { get; set; }
        public double Opening { get; set; }
        public double INQty { get; set; }
        public double OUTQty { get; set; }
        public double Closing { get; set; }
        public double Cost { get; set; }
        public double Value { get; set; }
    }
}
