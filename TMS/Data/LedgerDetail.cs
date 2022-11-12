using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class LedgerDetail
    {
        public long Id { get; set; }
        public long DocId { get; set; }
        public int DocTypeId { get; set; }
        public long? RefDocId { get; set; }
        public int? RefDocTypeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime PostDate { get; set; }
        public long AccountId { get; set; }
        public long? AgainstAccountId { get; set; }
        public long? ProductId { get; set; }
        [StringLength(500)]
        public string Narration { get; set; }
        [StringLength(5000)]
        public string NarrationDetailed { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public int TransactionStatusId { get; set; }
        public DateTime? DueDate { get; set; }
        public int? PaymentTermId { get; set; }
        public int? TradeFirmId { get; set; }
        public string UserId { get; set; }

        public virtual DocType DocType { get; set; }
        public virtual Account Account { get; set; }
        [ForeignKey("AgainstAccountId")]
        public virtual Account AgainstAccount { get; set; }
        public virtual Product Product { get; set; }
        public virtual TransactionStatus TransactionStatus { get; set; }
        public virtual PaymentTerm PaymentTerm { get; set; }
        public virtual TradeFirm TradeFirm { get; set; }
        public virtual User User { get; set; }
    }
}
