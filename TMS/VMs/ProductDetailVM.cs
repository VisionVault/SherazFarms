using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class ProductDetailVM
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Product { get; set; }
        public string Barcode { get; set; }
        public double CostPrice { get; set; }
        public double SalePrice { get; set; }
        public int UOMId { get; set; }
        public string UOM { get; set; }
        public double Packing { get; set; }
        public int MinQty { get; set; }
        public int Stock { get; set; }
    }
}
