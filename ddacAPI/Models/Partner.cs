using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ddacAPI.Models
{
    public class Partner
    {
        public int Id { get; set; }

        public Hotel Hotel { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
