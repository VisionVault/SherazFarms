using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class Account
    {
        public long Id { get; set; }
        public int GALId { get; set; }
        [Required, StringLength(20)]
        public string GALCode { get; set; }
        [Required, StringLength(30)]
        public string Code { get; set; }
        [Required, StringLength(250)]
        public string Name { get; set; }
        [StringLength(250)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int? TradeFirmId { get; set; }
        [StringLength(450)]
        public string UserId { get; set; }

        public virtual GAL GAL { get; set; }
        public virtual TradeFirm TradeFirm { get; set; }
        public virtual User User { get; set; }
    }
}
