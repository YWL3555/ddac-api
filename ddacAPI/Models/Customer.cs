using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ddacAPI.Models
{
    public class Customer : IdentityUser
    {

        public string Name { get; set; }

        public string ContactNumber { get; set; }

        public string ProfilePic { get; set; }
    }
}
