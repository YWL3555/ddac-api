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
using Microsoft.AspNetCore.Cors;

namespace ddacAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("MyPolicy")]
    [ApiController]
    public class RatingReviewController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly ddacAPIContext _context;

        public RatingReviewController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ddacAPIContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: api/RatingReview/ByHotel?id=2
        [HttpGet("ByHotel")]
        public object GetRatingReviewByHotel(int id)
        {
            int currentHotelId = id;

            var ratingReviews = _context.RatingReview.Where(r => r.HotelId == currentHotelId);

            if (ratingReviews == null)
            {
                return NotFound();
            }

            return ratingReviews;
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
        [HttpPost("PostRatingReview")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> PostRatingReview(int id, [FromBody] RatingReview ratingReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int currentHotelId = id;
            string customerId = User.Claims.First(c => c.Type == "UserID").Value;

            var ratingReviewToBeCreated = new RatingReview()
            {
                CustomerId = customerId,
                Rating = ratingReview.Rating,
                Review = ratingReview.Review,
                HotelId = currentHotelId
            };

            _context.RatingReview.Add(ratingReviewToBeCreated);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoomType", new { id = ratingReview.Id }, ratingReview);
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

        private bool RoomTypeExists(int id)
        {
            return _context.RoomType.Any(e => e.Id == id);
        }
    }
}