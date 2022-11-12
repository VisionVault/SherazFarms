using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class SaleReportVM
    {
        public bool IsGroupChanged { get; set; }
        public long Id { get; set; }
        public long DocId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Date { get; set; }
        public long? AccountId { get; set; }
        public string Account { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string VehicleNumber { get; set; }
        public string DriverName { get; set; }
        public string Remarks { get; set; }
        public long ProductId { get; set; }
        public string Product { get; set; }
        public int? LocationId { get; set; }
        public string Location { get; set; }
        public double Qty { get; set; }
        public double? EmptyWeight { get; set; }
        public double? LoadedWeight { get; set; }
        public double? ActualWeight { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
        public double? DiscountP { get; set; }
        public double Net { get; set; }
        public double? BillDiscount { get; set; }
        public double BillNet { get; set; }
        public double Payment { get; set; }
        public double? Delivery { get; set; }
    }
}
