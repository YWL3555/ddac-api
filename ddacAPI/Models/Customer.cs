using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ddacAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public string Email { get; set; }

        public string Password { get; set; }

        public string ContactNumber { get; set; }

        public string ProfilePic { get; set; }
    }
}
