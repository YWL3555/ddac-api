using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ddacAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public RoomType RoomType { get; set; }

        public int Quantity { get; set; }

        public Customer Customer { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Boolean Canceled { get; set; }

        public Boolean Reviewed { get; set; }
    }
}
