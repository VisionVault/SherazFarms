using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class STVM
    {
        public bool Print { get; set; }
        public long DocId { get; set; }
        public int DocTypeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public long OutProductId { get; set; }
        public string OutProductName { get; set; }
        public double OutQty { get; set; }
        public long InProductId { get; set; }
        public string InProductName { get; set; }
        public double InQty { get; set; }
        public double Cost { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
        public int? TFId { get; set; }
        public int TransactionStatusId { get; set; }
        public string UserId { get; set; }
    }
}
