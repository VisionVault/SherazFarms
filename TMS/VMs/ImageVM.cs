using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.VMs
{
    public class ImageVM
    {
        public long ImageId { get; set; }
        public long PropertyId { get; set; }
        public string PropertyName { get; set; }
        public IFormFile ImageFile { get; set; }
        public string Path { get; set; }
    }
}
