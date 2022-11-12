using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class User : IdentityUser
    {
        public int? TradeFirmId { get; set; }

        public virtual TradeFirm TradeFirm { get; set; }
    }
}
