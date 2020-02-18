﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ddacAPI.Models
{
    public class Admin
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("Id")]
        public virtual ApplicationUser User { get; set; }

    }
}
