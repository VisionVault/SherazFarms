using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class Product
    {
        public long Id { get; set; }
        public int? ProductCategoryId { get; set; }
        public int? BrandId { get; set; }
        [Required, StringLength(250)]
        public string Name { get; set; }
        public double CostPrice { get; set; }
        public double SalePrice { get; set; }
        public bool IsActive { get; set; }

        public virtual ProductCategory Category { get; set; }
        public virtual Brand Brand { get; set; }
    }
}
