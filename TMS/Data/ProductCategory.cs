﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class ProductCategory
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
