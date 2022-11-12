using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class TransactionStatus
    {
        public int Id { get; set; }
        [Required, StringLength(20)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
    }
}
