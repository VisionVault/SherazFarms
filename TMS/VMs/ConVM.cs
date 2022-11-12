using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class ConVM
    {
        public long DocId { get; set; }
        public int DocTypeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public long RawProductId { get; set; }
        public string RawProductName { get; set; }
        public double RawQty { get; set; }
        public double RawRate { get; set; }
        public double RawAmount { get; set; }
        public long ReadyProductId { get; set; }
        public string ReadyProductName { get; set; }
        public double ReadyQty { get; set; }
        public double ReadyRate { get; set; }
        public double ReadyAmount { get; set; }
        public int PaymentTermId { get; set; }
        public int? TFId { get; set; }
        public int TransactionStatusId { get; set; }
        public string UserId { get; set; }
    }
}
