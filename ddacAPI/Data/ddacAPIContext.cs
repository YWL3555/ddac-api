using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ddacAPI.Models;

namespace ddacAPI.Data
{
    public class ddacAPIContext : DbContext
    {
        public ddacAPIContext (DbContextOptions<ddacAPIContext> options)
            : base(options)
        {
        }

        public DbSet<ddacAPI.Models.Hotel> Hotel { get; set; }
    }
}
