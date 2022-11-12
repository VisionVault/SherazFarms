using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class RecipeVM
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Product { get; set; }
        public long RawMaterialId { get; set; }
        public string RawMaterial { get; set; }
        public double Qty { get; set; }
    }
}
