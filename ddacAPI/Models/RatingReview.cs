using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ddacAPI.Models
{
    public class RatingReview
    {
        public int Id { get; set; }

        public Customer Customer { get; set; }

        public int Rating { get; set; }

        public string Review { get; set; }

        public Hotel Hotel { get; set; }
    }
}
