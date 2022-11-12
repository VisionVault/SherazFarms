using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class ProductVM
    {
        public long Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public int? ProductCategoryId { get; set; }
        public string Category { get; set; }
        public int? BrandId { get; set; }
        public string Brand { get; set; }
        public string Name { get; set; }
        public string InitialName { get; set; }
        public double CostPrice { get; set; }
        public double SalePrice { get; set; }
        public double OpeningStock { get; set; }
        public double OpeningValue { get; set; }
        public int? TradeFirmId { get; set; }
        public int TransactionStatusId { get; set; }
        public bool IsActive { get; set; }
        public string UserId { get; set; }
    }
}
