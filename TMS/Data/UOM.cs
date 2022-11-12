using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class UOM
    {
        public int Id { get; set; }
        [Required, StringLength(250)]
        public string Name { get; set; }
    }
}
