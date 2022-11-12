using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class RPTInvoiceVM
    {
        public bool HR { get; set; }
        public long DocId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Date { get; set; }
        public string Term { get; set; }
        public string Account { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string VehicleNumber { get; set; }
        public string DriverName { get; set; }
        public string Product { get; set; }
        public string Location { get; set; }
        public double? EmptyWeight { get; set; }
        public double? LoadedWeight { get; set; }
        public double? ActualWeight { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
        public double? DiscountP { get; set; }
        public double? Discount { get; set; }
        public double Net { get; set; }
        public double? BillDiscountP { get; set; }
        public double? BillDiscount { get; set; }
        public double BillNet { get; set; }
        public double Payment { get; set; }
    }
}
