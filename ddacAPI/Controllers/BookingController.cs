using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ddacAPI.Data;
using ddacAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ddacAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly ddacAPIContext _context;

        public BookingController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ddacAPIContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: api/RatingReview/ByHotel?id=2
        [HttpGet("ByHotel")]
        public object GetBookings(int id)
        {
            var currentHotelId = id;

            string customerId = User.Claims.First(c => c.Type == "UserID").Value;

            var bookings = _context.Booking.Include(b => b.RoomType).Where(b => b.RoomType.HotelId == currentHotelId);

            if (bookings == null)
            {
                return NotFound();
            }

            return bookings;
        }

        // GET: api/RatingReview/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roomType = await _context.RoomType.FindAsync(id);

            if (roomType == null)
            {
                return NotFound();
            }

            return Ok(roomType);
        }

       
        // POST: api/RoomTypes
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> PostBooking([FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string customerId = User.Claims.First(c => c.Type == "UserID").Value;

            var room = await _context.RoomType.FindAsync(booking.RoomTypeId);

            var days = (booking.EndDate - booking.StartDate).TotalDays;

            booking.TotalAmount = room.Price * Convert.ToDecimal(days);

            var bookingToBeCreated = new Booking()
            {
                CustomerId = customerId,
                RoomTypeId = booking.RoomTypeId,
                TotalAmount = booking.TotalAmount,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                Reviewed = false,
                Canceled = false
            };

            _context.Booking.Add(bookingToBeCreated);
            await _context.SaveChangesAsync();

            return Ok(bookingToBeCreated);
        }

        // POST: api/Booking/cancel?id=1
        [HttpPut("cancel")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int currentBookingId = id;

            string currentCustomerId = User.Claims.First(c => c.Type == "UserID").Value;

            var booking = await _context.Booking.FindAsync(currentBookingId);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.CustomerId != currentCustomerId)
            {
                return Forbid();
            }

            booking.Canceled = true;

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


            return Ok();
        }

        // POST: api/Booking/reviewed?id=1
        [HttpPut("reviewed")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ReviewedBooking(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int currentBookingId = id;

            string currentCustomerId = User.Claims.First(c => c.Type == "UserID").Value;

            var booking = await _context.Booking.FindAsync(currentBookingId);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.CustomerId != currentCustomerId)
            {
                return Forbid();
            }

            booking.Reviewed = true;

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


            return Ok();
        }


        // DELETE: api/RoomTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roomType = await _context.RoomType.FindAsync(id);
            if (roomType == null)
            {
                return NotFound();
            }

            _context.RoomType.Remove(roomType);
            await _context.SaveChangesAsync();

            return Ok(roomType);
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.Id == id);
        }
    }
}