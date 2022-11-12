using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class TradeFirm
    {
        public int Id { get; set; }
        [StringLength(250), Required]
        public string Name { get; set; }
        [StringLength(50)]
        public string Contact { get; set; }
        [StringLength(250)]
        public string Email { get; set; }
        [StringLength(5000)]
        public string Address { get; set; }
    }
}
