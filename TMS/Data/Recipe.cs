using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class Recipe
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long RawMaterialId { get; set; }
        public double Qty { get; set; }

        public virtual Product Product { get; set; }
        [ForeignKey("RawMaterialId")]
        public virtual Product RawMaterial { get; set; }
    }
}
