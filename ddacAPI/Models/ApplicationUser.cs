using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ddacAPI.Models
{
    public class ApplicationUser : IdentityUser
    {

        public virtual Customer Customer { get; set; }
        public virtual Partner Partner { get; set; }
        public virtual Admin Admin { get; set; }
    }
}
