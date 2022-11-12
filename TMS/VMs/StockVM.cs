using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class StockVM
    {
        public bool IsDyeingChanged { get; set; }
        public long Id { get; set; }
        public long? AccountId { get; set; }
        public string Account { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public long ProductId { get; set; }
        public string LotNumber { get; set; }
        public string Product { get; set; }
        public string Barcode { get; set; }
        public double Qty { get; set; }
        public double Opening { get; set; }
        public double Inward { get; set; }
        public double Outward { get; set; }
        public double Closing { get; set; }
    }
}
