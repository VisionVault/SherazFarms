using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class GAL
    {
        public int Id { get; set; }
        public int CTLId { get; set; }
        [Required, StringLength(10)]
        public string CTLCode { get; set; }
        [Required, StringLength(20)]
        public string Code { get; set; }
        [Required, StringLength(250)]
        public string Name { get; set; }

        public virtual CTL CTL { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
