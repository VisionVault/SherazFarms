using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class DocVM
    {
        public bool Print { get; set; }
        public long DocId { get; set; }
        public int DocTypeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Remarks { get; set; }
        public string ReceiptNumber { get; set; }
        public string VehicleNumber { get; set; }
        public string DriverName { get; set; }
        public long AccountId { get; set; }
        public long? BankAcId { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string RegistrationNumber { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public int? LocationId { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public double EmptyWeight { get; set; }
        public double LoadedWeight { get; set; }
        public double ActualWeight { get; set; }
        public double Cost { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
        public double DiscountP { get; set; }
        public double Discount { get; set; }
        public double Net { get; set; }
        public double BillDiscountP { get; set; }
        public double BillDiscount { get; set; }
        public double Payment { get; set; }
        public DateTime? DueDate { get; set; }
        public int PaymentTermId { get; set; }
        public int? TFId { get; set; }
        public int TransactionStatusId { get; set; }
        public string UserId { get; set; }
    }
}
