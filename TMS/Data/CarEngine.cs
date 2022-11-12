using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class CarEngine
    {
        public int Id { get; set; }
        public int? CarBrandId { get; set; }
        [StringLength(500)]
        public string EngineNumber { get; set; }
        [StringLength(50)]
        public string EngineCC { get; set; }
        [StringLength(50)]
        public string OilCapacity { get; set; }
        [StringLength(500)]
        public string Company { get; set; }
        [StringLength(100)]
        public string OilFilter { get; set; }
        [StringLength(100)]
        public string AirFilter { get; set; }
        [StringLength(100)]
        public string FuelFilter { get; set; }
        [StringLength(100)]
        public string CabinFilter { get; set; }

        public virtual CarBrand CarBrand { get; set; }
    }
}
