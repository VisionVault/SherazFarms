using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class CarEngineVM
    {
        public int Id { get; set; }
        public int? CarBrandId { get; set; }
        public string EngineNumber { get; set; }
        public string InitialEngineNumber { get; set; }
        public string EngineCC { get; set; }
        public string OilCapacity { get; set; }
        public string Company { get; set; }
        public string OilFilter { get; set; }
        public string AirFilter { get; set; }
        public string FuelFilter { get; set; }
        public string CabinFilter { get; set; }
    }
}
