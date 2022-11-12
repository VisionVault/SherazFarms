using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class CarBrand
    {
        public int Id { get; set; }
        public int? CarId { get; set; }
        [Required, StringLength(250)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Model { get; set; }
        [StringLength(50)]
        public string Year { get; set; }

        public virtual Car Car { get; set; }
        public virtual List<CarEngine> CarEngines { get; set; }
    }
}
