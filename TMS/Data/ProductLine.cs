using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class ProductLine
    {
        public long Id { get; set; }
        public long DocId { get; set; }
        public int DocTypeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime PostDate { get; set; }
        [StringLength(500)]
        public string Remarks { get; set; }
        [StringLength(250)]
        public string VehicleNumber { get; set; }
        [StringLength(250)]
        public string DriverName { get; set; }
        [StringLength(50)]
        public string ReceiptNumber { get; set; }
        public long? AccountId { get; set; }
        public double? EmptyWeight { get; set; }
        public double? LoadedWeight { get; set; }
        public double? ActualWeight { get; set; }
        public long ProductId { get; set; }
        public double Stock { get; set; }
        public double StockValue { get; set; }
        public double Qty { get; set; }
        public double Cost { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
        public double? DiscountP { get; set; }
        public double? Discount { get; set; }
        public double Net { get; set; }
        public double? BillDiscountP { get; set; }
        public double? BillDiscount { get; set; }
        public double Payment { get; set; }
        public int? LocationId { get; set; }
        public int TransactionStatusId { get; set; }
        public int? TradeFirmId { get; set; }
        public int? PaymentTermId { get; set; }
        public string UserId { get; set; }

        public virtual DocType DocType { get; set; }
        public virtual Account Account { get; set; }
        public virtual Product Product { get; set; }
        public virtual TransactionStatus TransactionStatus { get; set; }
        public virtual Location Location { get; set; }
        public virtual PaymentTerm PaymentTerm { get; set; }
        public virtual TradeFirm TradeFirm { get; set; }
        public virtual User User { get; set; }
    }
}
