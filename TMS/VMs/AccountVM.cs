using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class AccountVM
    {
        public long Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string GALCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string CNIC { get; set; }
        public double Balance { get; set; }
        public int TradeFirmId { get; set; }
        public int TransactionStatusId { get; set; }
        public string UserId { get; set; }
        public long CustomerCarId { get; set; }
        public string RegistrationNumber { get; set; }
        public string Color { get; set; }
    }
}
