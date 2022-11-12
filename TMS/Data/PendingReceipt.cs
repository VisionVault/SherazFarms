using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class PendingReceipt
    {
        public long Id { get; set; }
        public long DocId { get; set; }
        public int DocTypeId { get; set; }
    }
}
