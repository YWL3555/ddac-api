using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ddacAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ddacAPI.Data
{
    public class ddacAPIContext : IdentityDbContext
    {
        public ddacAPIContext (DbContextOptions<ddacAPIContext> options)
            : base(options)
        {
        }

        public DbSet<ddacAPI.Models.Hotel> Hotel { get; set; }

        public DbSet<ddacAPI.Models.Partner> Partner { get; set; }

        public DbSet<ddacAPI.Models.RoomType> RoomType { get; set; }

        public DbSet<ddacAPI.Models.Booking> Booking { get; set; }

        public DbSet<ddacAPI.Models.RatingReview> RatingReview { get; set; }

        public DbSet<ddacAPI.Models.ApplicationUser> ApplicationUser { get; set; }

        public DbSet<ddacAPI.Models.Customer> Customer { get; set; }

        public DbSet<ddacAPI.Models.Admin> Admin { get; set; }

        public DbSet<ddacAPI.Models.Facility> Facility { get; set; }

        public DbSet<ddacAPI.Models.City> City { get; set; }

        public DbSet<ddacAPI.Models.State> State { get; set; }
    }
}
