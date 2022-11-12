using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class CustomerCar
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        [StringLength(250)]
        public string RegistrationNumber { get; set; }
        [StringLength(50)]
        public string Color { get; set; }

        public virtual Account Account { get; set; }
    }
}
