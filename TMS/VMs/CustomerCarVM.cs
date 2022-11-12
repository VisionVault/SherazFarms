using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class CustomerCarVM
    {
        public int Id { get; set; }
        public long AccountId { get; set; }
        public string RegistrationNumber { get; set; }
        public string InitialRegistrationNumber { get; set; }
        public string Color { get; set; }
    }
}
