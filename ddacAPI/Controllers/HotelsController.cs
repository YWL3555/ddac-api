using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ddacAPI.Data;
using ddacAPI.Models;
using Microsoft.AspNetCore.Authorization;
using ddacAPI.Util;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Dynamic;

namespace ddacAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly ddacAPIContext _context;
        private static BlobManager _blobManager;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;

        public IConfiguration Configuration { get; }

        public HotelsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ddacAPIContext context, IConfiguration configuration)
        {
            Configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: api/Hotels/search?city=ipoh&startdate=01/02/2020&enddate=01/02/2020
        [HttpGet("search")]
        public IEnumerable<Hotel> SearchHotels(string city, DateTime startdate, DateTime enddate)
        {
            var hotels = _context.Hotel.Where(h => h.Published == true);

            return hotels;
        }

        // GET: api/Hotels
        [HttpGet]
        public ICollection<ExpandoObject> GetHotel()
        {
            var hotels = _context.Hotel.Where(h => h.Published == true);

            List<ExpandoObject> hList = new List<ExpandoObject>();

            foreach (var hotel in hotels)
            {
                dynamic hotelWithRoomTypes = new ExpandoObject();
                hotelWithRoomTypes.hotel = hotel;
                hotelWithRoomTypes.roomTypes =  _context.RoomType.Where(r => r.HotelId == hotel.Id);
                hotelWithRoomTypes.ratingReviews = _context.RatingReview.Where(r => r.HotelId == hotel.Id);
                hList.Add(hotelWithRoomTypes);
            }

            return hList; 
        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotel([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hotel = await _context.Hotel.FindAsync(id);

            var ratingreviews = _context.RatingReview.Include(r => r.Customer).Where(r => r.HotelId == id);

            dynamic hotelForAPI = new ExpandoObject();

            hotelForAPI.hotel = hotel;

            hotelForAPI.ratingreviews = ratingreviews;
            
            hotelForAPI.meme = "finally boiii";

            if (hotel == null)
            {
                return NotFound();
            }

            return Ok(hotelForAPI
            );
        }

        // PUT: api/Hotels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel([FromRoute] int id, [FromBody] Hotel hotel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != hotel.Id)
            {
                return BadRequest();
            }

            _context.Entry(hotel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/Hotels/publish?id=1
        [HttpPut("publish")]
        public async Task<IActionResult> PublishHotel(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var hotel = await _context.Hotel.FindAsync(id);

            if (hotel == null)
            {
                return NotFound();
            }

            hotel.Published = true;

            _context.Entry(hotel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/Hotels/publish?id=1
        [HttpPut("unpublish")]
        public async Task<IActionResult> UnpublishHotel(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hotel = await _context.Hotel.FindAsync(id);

            if (hotel == null)
            {
                return NotFound();
            }

            hotel.Published = true;

            _context.Entry(hotel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Hotels
        [HttpPost]
        public async Task<IActionResult> PostHotel([FromBody] Hotel hotel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Hotel.Add(hotel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hotel = await _context.Hotel.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            _context.Hotel.Remove(hotel);
            await _context.SaveChangesAsync();

            return Ok(hotel);
        }

        [HttpPost]
        [Authorize(Roles = "Partner")]
        [Route("upload")]
        //GET: /api/hotels/upload
        public async Task<IActionResult> UploadUserProfilePic(IFormFile newFile)
        {
            _blobManager = new BlobManager(Configuration);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.Claims.First(c => c.Type == "UserID").Value;

            var partner = await _context.Partner.FindAsync(userId);

            var hotel = await _context.Hotel.FindAsync(partner.HotelId);


            if (newFile != null)
            {
                Stream filestreamFromRequest = newFile.OpenReadStream();
                hotel.Photo = await _blobManager.UploadFileToStorageAsync(filestreamFromRequest, "hotel_"+hotel.Id.ToString() + ".jpg");
            }
            else
            {
                return BadRequest();
            }


            _context.Entry(hotel).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();

        }

        private bool HotelExists(int id)
        {
            return _context.Hotel.Any(e => e.Id == id);
        }
    }
}