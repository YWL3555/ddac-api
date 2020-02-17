using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ddacAPI.Models
{
    public class RoomType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string Photo { get; set; }

        public int MaximumPax { get; set; }

        public Hotel Hotel { get; set; }
    }
}
