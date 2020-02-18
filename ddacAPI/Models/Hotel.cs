using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ddacAPI.Models
{
    public class Hotel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ContactNumber { get; set; }

        public string Photo { get; set; }

        public string Facilities { get; set; }

        public Boolean Published { get; set; }
    }
}
