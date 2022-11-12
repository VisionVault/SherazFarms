using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class AccountDetail
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        [StringLength(50)]
        public string CNIC { get; set; }
        [StringLength(50)]
        public string Contact { get; set; }
        [StringLength(250)]
        public string Email { get; set; }
        [StringLength(500)]
        public string AccountNumber { get; set; }
        [StringLength(5000)]
        public string Address { get; set; }
        public int? CityId { get; set; }

        public virtual City City { get; set; }
        public virtual Account Account { get; set; }
    }
}
