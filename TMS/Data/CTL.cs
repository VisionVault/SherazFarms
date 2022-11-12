using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class CTL
    {
        public int Id { get; set; }
        public int FSLId { get; set; }
        [Required, StringLength(10)]
        public string FSLCode { get; set; }
        [Required, StringLength(10)]
        public string Code { get; set; }
        [Required, StringLength(250)]
        public string Name { get; set; }

        public virtual FSL FSL { get; set; }
        public virtual ICollection<GAL> GALs { get; set; }
    }
}
